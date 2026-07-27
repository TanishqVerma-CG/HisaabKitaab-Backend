namespace BillingSystem.Models;

public class CustomerProductPrice
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int ProductId { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Party Customer { get; set; }
    public Product Product { get; set; }
}