using BillingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BillingSystem.Data.Repositories;

public class PartyRepository : Repository<Party>, IPartyRepository
{
    public PartyRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Party>> GetCustomersAsync()
    {
        return await _dbSet.Where(p => p.IsCustomer).ToListAsync();
    }
}