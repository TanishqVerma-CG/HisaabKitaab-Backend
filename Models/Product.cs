namespace BillingSystem.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string HsnCode { get; set; }
    public decimal BasePrice { get; set; }
    public decimal GstRate { get; set; }
    public string Unit { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<CustomerProductPrice> CustomerProductPrices { get; set; }
    public ICollection<BillItem> BillItems { get; set; }
}