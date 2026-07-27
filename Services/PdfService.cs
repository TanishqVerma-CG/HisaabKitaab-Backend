using BillingSystem.Models;
using BillingSystem.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Microsoft.Extensions.Configuration;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace BillingSystem.Services;

public class PdfService : IPdfService
{
    private readonly string _pdfDirectory;
    private readonly string _signaturePath;
    private readonly IConfiguration _configuration;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName = "pdfs";
    private readonly ILogger<PdfService> _logger;

    public PdfService(IConfiguration configuration, ILogger<PdfService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _pdfDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdfs");
        _signaturePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "signature.png");

        if (!Directory.Exists(_pdfDirectory))
            Directory.CreateDirectory(_pdfDirectory);

        QuestPDF.Settings.License = LicenseType.Community;

        var azureStorageConnectionString = _configuration["AzureStorage:ConnectionString"];

        if (string.IsNullOrEmpty(azureStorageConnectionString))
        {
            _logger.LogWarning("╔══════════════════════════════════════════════════════════════╗");
            _logger.LogWarning("║  ⚠️  AZURE BLOB STORAGE NOT CONFIGURED!                      ║");
            _logger.LogWarning("║  PDFs will be stored locally and LOST on container restart  ║");
            _logger.LogWarning("║  Set environment variable: AzureStorage__ConnectionString   ║");
            _logger.LogWarning("╚══════════════════════════════════════════════════════════════╝");
        }
        else
        {
            _blobServiceClient = new BlobServiceClient(azureStorageConnectionString);
            _logger.LogInformation("✓ Azure Blob Storage configured successfully");
        }
    }

    public async Task<string> GenerateBillPdfAsync(Bill bill)
    {
        _logger.LogInformation($"Starting PDF generation for Bill: {bill.BillNumber}");
        _logger.LogInformation($"Customer: {bill.Customer?.Name ?? "NULL"}");
        _logger.LogInformation($"Bill Items Count: {bill.BillItems?.Count ?? 0}");

        if (bill.Customer == null)
        {
            _logger.LogError("Customer is NULL! Cannot generate PDF.");
            throw new Exception("Bill customer data is missing");
        }

        if (bill.BillItems == null || !bill.BillItems.Any())
        {
            _logger.LogError("Bill items are NULL or empty! Cannot generate PDF.");
            throw new Exception("Bill items data is missing");
        }

        var fileName = $"{bill.BillNumber}.pdf";
        var filePath = Path.Combine(_pdfDirectory, fileName);

        var shopName = _configuration["ShopDetails:Name"] ?? "Your Shop Name";
        var shopGst = _configuration["ShopDetails:GstNumber"] ?? "GST000000000";

        _logger.LogInformation($"Generating PDF with shop: {shopName}");

        try
        {
            await Task.Run(() =>
            {
                Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(40);

                        page.Header().Column(column =>
                        {
                            column.Item().AlignCenter().Text(shopName).FontSize(20).Bold();
                            column.Item().AlignCenter().Text($"GST: {shopGst}").FontSize(10);
                            column.Item().PaddingVertical(10).LineHorizontal(1);
                        });

                        page.Content().Column(column =>
                        {
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text($"Bill No: {bill.BillNumber}").FontSize(10);
                                col.Item().Text($"Date: {bill.BillDate:dd/MM/yyyy}").FontSize(10);
                            });

                            row.RelativeItem().Column(col =>
                            {
                                col.Item().AlignRight().Text($"Customer: {bill.Customer.Name}").FontSize(10);
                                col.Item().AlignRight().Text($"GST: {bill.Customer.GstNumber ?? "N/A"}").FontSize(10);
                            });
                        });

                        column.Item().PaddingVertical(10);

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(30);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("#");
                                header.Cell().Element(CellStyle).Text("Product");
                                header.Cell().Element(CellStyle).Text("HSN");
                                header.Cell().Element(CellStyle).Text("Qty");
                                header.Cell().Element(CellStyle).Text("Rate");
                                header.Cell().Element(CellStyle).Text("GST%");
                                header.Cell().Element(CellStyle).Text("Amount");
                            });

                            var index = 1;
                            foreach (var item in bill.BillItems)
                            {
                                table.Cell().Element(CellStyle).Text(index++.ToString());
                                table.Cell().Element(CellStyle).Text(item.Product.Name);
                                table.Cell().Element(CellStyle).Text(item.Product.HsnCode ?? "N/A");
                                table.Cell().Element(CellStyle).AlignRight().Text(item.Quantity.ToString("N2"));
                                table.Cell().Element(CellStyle).AlignRight().Text(item.Rate.ToString("N2"));
                                table.Cell().Element(CellStyle).AlignRight().Text(item.GstRate.ToString("N2"));
                                table.Cell().Element(CellStyle).AlignRight().Text(item.Amount.ToString("N2"));
                            }
                        });

                        column.Item().PaddingVertical(10);

                        column.Item().AlignRight().Column(col =>
                        {
                            col.Item().Text($"Subtotal: ₹{bill.Subtotal:N2}").FontSize(10);
                            col.Item().Text($"GST Amount: ₹{bill.GstAmount:N2}").FontSize(10);
                            col.Item().Text($"Grand Total: ₹{bill.GrandTotal:N2}").FontSize(12).Bold();
                        });

                        column.Item().PaddingTop(30);

                        if (File.Exists(_signaturePath))
                        {
                            column.Item().AlignRight().Width(100).Height(50).Image(_signaturePath);
                        }

                        column.Item().AlignRight().Text("Authorized Signature").FontSize(8);
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Page ");
                        text.CurrentPageNumber();
                    });
                });
            }).GeneratePdf(filePath);
            });

            _logger.LogInformation($"PDF file generated successfully at: {filePath}");

            if (File.Exists(filePath))
            {
                var fileInfo = new FileInfo(filePath);
                _logger.LogInformation($"PDF file size: {fileInfo.Length} bytes");
            }
            else
            {
                _logger.LogError("PDF file was not created!");
                throw new Exception("PDF generation failed - file not created");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error during PDF generation for {bill.BillNumber}");
            throw;
        }

        // Check if Azure Blob Storage is configured
        if (_blobServiceClient == null)
        {
            _logger.LogWarning("Azure Blob Storage is NOT configured! PDFs will be stored locally and will be lost on restart.");
            _logger.LogWarning("Please set the AzureStorage__ConnectionString environment variable.");
        }

        if (_blobServiceClient != null)
        {
            try
            {
                _logger.LogInformation($"Uploading PDF to Azure Blob Storage: {fileName}");
                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

                var blobClient = containerClient.GetBlobClient(fileName);

                using (var fileStream = File.OpenRead(filePath))
                {
                    _logger.LogInformation($"Uploading PDF file, size: {fileStream.Length} bytes");
                    await blobClient.UploadAsync(fileStream, overwrite: true);
                }

                _logger.LogInformation($"✓ PDF successfully uploaded to Azure Blob Storage: {fileName}");

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                return fileName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"✗ Failed to upload PDF to Azure Blob Storage: {fileName}");
                _logger.LogWarning("Falling back to local storage (PDFs will be lost on restart!)");
                return Path.Combine("pdfs", fileName);
            }
        }

        _logger.LogWarning($"PDF stored locally (will be lost on restart): {filePath}");
        return Path.Combine("pdfs", fileName);
    }

    public async Task<byte[]> GetBillPdfAsync(string pdfPath)
    {
        if (_blobServiceClient != null)
        {
            try
            {
                var fileName = Path.GetFileName(pdfPath);
                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                var blobClient = containerClient.GetBlobClient(fileName);

                if (!await blobClient.ExistsAsync())
                {
                    _logger.LogWarning($"PDF not found in Azure Blob Storage: {fileName}");
                    throw new FileNotFoundException($"PDF file not found: {fileName}");
                }

                using (var memoryStream = new MemoryStream())
                {
                    await blobClient.DownloadToAsync(memoryStream);
                    _logger.LogInformation($"PDF downloaded from Azure Blob Storage: {fileName}");
                    return memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error downloading PDF from Azure Blob Storage: {pdfPath}");
                throw;
            }
        }

        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", pdfPath);

        if (!File.Exists(fullPath))
        {
            _logger.LogWarning($"PDF not found in local storage: {fullPath}");
            throw new FileNotFoundException("PDF file not found");
        }

        return await File.ReadAllBytesAsync(fullPath);
    }

    private static IContainer CellStyle(IContainer container)
    {
        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
    }
}