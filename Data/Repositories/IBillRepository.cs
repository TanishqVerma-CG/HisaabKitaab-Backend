using BillingSystem.Models;

namespace BillingSystem.Data.Repositories;

public interface IBillRepository : IRepository<Bill>
{
    Task<Bill> GetBillWithDetailsAsync(int id);
    Task<IEnumerable<Bill>> GetAllWithDetailsAsync();
    Task<string> GenerateNextBillNumberAsync();
}