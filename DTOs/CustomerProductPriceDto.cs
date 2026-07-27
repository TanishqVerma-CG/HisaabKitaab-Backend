namespace BillingSystem.DTOs;

public class CustomerProductPriceDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
}

public class CreateCustomerProductPriceDto
{
    public int CustomerId { get; set; }
    public int ProductId { get; set; }
    public decimal Price { get; set; }
}

public class UpdateCustomerProductPriceDto
{
    public decimal Price { get; set; }
}