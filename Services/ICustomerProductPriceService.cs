using BillingSystem.DTOs;

namespace BillingSystem.Services;

public interface ICustomerProductPriceService
{
    Task<IEnumerable<CustomerProductPriceDto>> GetPricesByCustomerAsync(int customerId);
    Task<CustomerProductPriceDto> CreatePriceAsync(CreateCustomerProductPriceDto dto);
    Task<CustomerProductPriceDto> UpdatePriceAsync(int id, UpdateCustomerProductPriceDto dto);
    Task DeletePriceAsync(int id);
}