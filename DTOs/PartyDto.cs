namespace BillingSystem.DTOs;

public class PartyDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string GstNumber { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public bool IsCustomer { get; set; }
    public bool IsSupplier { get; set; }
}

public class CreatePartyDto
{
    public string Name { get; set; }
    public string GstNumber { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public bool IsCustomer { get; set; }
    public bool IsSupplier { get; set; }
}

public class UpdatePartyDto
{
    public string Name { get; set; }
    public string GstNumber { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public bool IsCustomer { get; set; }
    public bool IsSupplier { get; set; }
}