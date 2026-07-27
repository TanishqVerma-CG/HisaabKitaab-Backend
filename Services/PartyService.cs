using AutoMapper;
using BillingSystem.Data.Repositories;
using BillingSystem.DTOs;
using BillingSystem.Models;

namespace BillingSystem.Services;

public class PartyService : IPartyService
{
    private readonly IPartyRepository _repository;
    private readonly IMapper _mapper;

    public PartyService(IPartyRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PartyDto>> GetAllPartiesAsync()
    {
        var parties = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<PartyDto>>(parties);
    }

    public async Task<PartyDto> GetPartyByIdAsync(int id)
    {
        var party = await _repository.GetByIdAsync(id);
        if (party == null)
            throw new Exception("Party not found");

        return _mapper.Map<PartyDto>(party);
    }

    public async Task<PartyDto> CreatePartyAsync(CreatePartyDto dto)
    {
        var party = _mapper.Map<Party>(dto);
        await _repository.AddAsync(party);
        return _mapper.Map<PartyDto>(party);
    }

    public async Task<PartyDto> UpdatePartyAsync(int id, UpdatePartyDto dto)
    {
        var party = await _repository.GetByIdAsync(id);
        if (party == null)
            throw new Exception("Party not found");

        _mapper.Map(dto, party);
        party.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(party);

        return _mapper.Map<PartyDto>(party);
    }

    public async Task DeletePartyAsync(int id)
    {
        var party = await _repository.GetByIdAsync(id);
        if (party == null)
            throw new Exception("Party not found");

        await _repository.DeleteAsync(party);
    }
}