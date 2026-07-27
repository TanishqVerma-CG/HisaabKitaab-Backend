namespace BillingSystem.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string HsnCode { get; set; }
    public decimal BasePrice { get; set; }
    public decimal GstRate { get; set; }
    public string Unit { get; set; }
}

public class CreateProductDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string HsnCode { get; set; }
    public decimal BasePrice { get; set; }
    public decimal GstRate { get; set; }
    public string Unit { get; set; }
}

public class UpdateProductDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string HsnCode { get; set; }
    public decimal BasePrice { get; set; }
    public decimal GstRate { get; set; }
    public string Unit { get; set; }
}