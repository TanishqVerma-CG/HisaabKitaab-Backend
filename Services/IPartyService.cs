using BillingSystem.DTOs;

namespace BillingSystem.Services;

public interface IPartyService
{
    Task<IEnumerable<PartyDto>> GetAllPartiesAsync();
    Task<PartyDto> GetPartyByIdAsync(int id);
    Task<PartyDto> CreatePartyAsync(CreatePartyDto dto);
    Task<PartyDto> UpdatePartyAsync(int id, UpdatePartyDto dto);
    Task DeletePartyAsync(int id);
}