using BillingSystem.Models;

namespace BillingSystem.Data.Repositories;

public interface IPartyRepository : IRepository<Party>
{
    Task<IEnumerable<Party>> GetCustomersAsync();
}