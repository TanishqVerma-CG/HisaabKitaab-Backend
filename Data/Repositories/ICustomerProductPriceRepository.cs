using BillingSystem.Models;

namespace BillingSystem.Data.Repositories;

public interface ICustomerProductPriceRepository : IRepository<CustomerProductPrice>
{
    Task<IEnumerable<CustomerProductPrice>> GetByCustomerIdAsync(int customerId);
    Task<CustomerProductPrice> GetByCustomerAndProductAsync(int customerId, int productId);
}