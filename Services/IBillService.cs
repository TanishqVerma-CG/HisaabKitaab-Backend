using BillingSystem.DTOs;

namespace BillingSystem.Services;

public interface IBillService
{
    Task<IEnumerable<BillDto>> GetAllBillsAsync();
    Task<BillDto> GetBillByIdAsync(int id);
    Task<BillDto> CreateBillAsync(CreateBillDto dto);
}