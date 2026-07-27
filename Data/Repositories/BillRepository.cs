using BillingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BillingSystem.Data.Repositories;

public class BillRepository : Repository<Bill>, IBillRepository
{
    public BillRepository(AppDbContext context) : base(context) { }

    public async Task<Bill> GetBillWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(b => b.Customer)
            .Include(b => b.BillItems)
            .ThenInclude(bi => bi.Product)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<Bill>> GetAllWithDetailsAsync()
    {
        return await _dbSet
            .Include(b => b.Customer)
            .Include(b => b.BillItems)
            .OrderByDescending(b => b.BillDate)
            .ToListAsync();
    }

    public async Task<string> GenerateNextBillNumberAsync()
    {
        var lastBill = await _dbSet
            .OrderByDescending(b => b.Id)
            .FirstOrDefaultAsync();

        if (lastBill == null)
            return $"BILL-{DateTime.Now.Year}-0001";

        var lastNumber = int.Parse(lastBill.BillNumber.Split('-').Last());
        var newNumber = lastNumber + 1;
        return $"BILL-{DateTime.Now.Year}-{newNumber:D4}";
    }
}