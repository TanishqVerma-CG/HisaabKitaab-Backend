namespace BillingSystem.Models;

public class BillItem
{
    public int Id { get; set; }
    public int BillId { get; set; }
    public int ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }
    public decimal GstRate { get; set; }

    public Bill Bill { get; set; }
    public Product Product { get; set; }
}