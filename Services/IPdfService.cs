using BillingSystem.Models;

namespace BillingSystem.Services;

public interface IPdfService
{
    Task<string> GenerateBillPdfAsync(Bill bill);
    Task<byte[]> GetBillPdfAsync(string pdfPath);
}