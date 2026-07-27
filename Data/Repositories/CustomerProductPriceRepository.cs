using BillingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BillingSystem.Data.Repositories;

public class CustomerProductPriceRepository : Repository<CustomerProductPrice>, ICustomerProductPriceRepository
{
    public CustomerProductPriceRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<CustomerProductPrice>> GetByCustomerIdAsync(int customerId)
    {
        return await _dbSet
            .Include(cp => cp.Customer)
            .Include(cp => cp.Product)
            .Where(cp => cp.CustomerId == customerId)
            .ToListAsync();
    }

    public async Task<CustomerProductPrice> GetByCustomerAndProductAsync(int customerId, int productId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(cp => cp.CustomerId == customerId && cp.ProductId == productId);
    }
}