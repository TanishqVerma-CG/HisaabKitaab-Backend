using BillingSystem.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace BillingSystem.Services;

public class PdfService : IPdfService
{
    private readonly string _pdfDirectory;
    private readonly string _signaturePath;
    private readonly IConfiguration _configuration;

    public PdfService(IConfiguration configuration)
    {
        _configuration = configuration;
        _pdfDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdfs");
        _signaturePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "signature.png");

        if (!Directory.Exists(_pdfDirectory))
            Directory.CreateDirectory(_pdfDirectory);

        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<string> GenerateBillPdfAsync(Bill bill)
    {
        var fileName = $"{bill.BillNumber}.pdf";
        var filePath = Path.Combine(_pdfDirectory, fileName);

        var shopName = _configuration["ShopDetails:Name"] ?? "Your Shop Name";
        var shopGst = _configuration["ShopDetails:GstNumber"] ?? "GST000000000";

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

        return Path.Combine("pdfs", fileName);
    }

    public async Task<byte[]> GetBillPdfAsync(string pdfPath)
    {
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", pdfPath);

        if (!File.Exists(fullPath))
            throw new Exception("PDF file not found");

        return await File.ReadAllBytesAsync(fullPath);
    }

    private static IContainer CellStyle(IContainer container)
    {
        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
    }
}