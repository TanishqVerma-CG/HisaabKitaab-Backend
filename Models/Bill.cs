namespace BillingSystem.Models;

public class Bill
{
    public int Id { get; set; }
    public string BillNumber { get; set; }
    public int CustomerId { get; set; }
    public DateTime BillDate { get; set; }
    public decimal Subtotal { get; set; }
    public decimal GstAmount { get; set; }
    public decimal GrandTotal { get; set; }
    public string PdfPath { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Party Customer { get; set; }
    public ICollection<BillItem> BillItems { get; set; }
}