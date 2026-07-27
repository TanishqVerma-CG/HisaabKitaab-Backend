namespace BillingSystem.Models;

public class Party
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string GstNumber { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public bool IsCustomer { get; set; }
    public bool IsSupplier { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<CustomerProductPrice> CustomerProductPrices { get; set; }
    public ICollection<Bill> Bills { get; set; }
}