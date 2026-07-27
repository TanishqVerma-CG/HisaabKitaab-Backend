namespace BillingSystem.DTOs;

public class BillDto
{
    public int Id { get; set; }
    public string BillNumber { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; }
    public string CustomerGstNumber { get; set; }
    public DateTime BillDate { get; set; }
    public decimal Subtotal { get; set; }
    public decimal GstAmount { get; set; }
    public decimal GrandTotal { get; set; }
    public string PdfPath { get; set; }
    public List<BillItemDto> BillItems { get; set; }
}

public class BillItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public string HsnCode { get; set; }
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }
    public decimal GstRate { get; set; }
}

public class CreateBillDto
{
    public int CustomerId { get; set; }
    public DateTime BillDate { get; set; }
    public List<CreateBillItemDto> BillItems { get; set; }
}

public class CreateBillItemDto
{
    public int ProductId { get; set; }
    public decimal Quantity { get; set; }
}