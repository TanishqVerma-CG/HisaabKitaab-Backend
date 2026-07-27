using Microsoft.AspNetCore.Mvc;
using BillingSystem.DTOs;
using BillingSystem.Services;

namespace BillingSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PartiesController : ControllerBase
{
    private readonly IPartyService _partyService;

    public PartiesController(IPartyService partyService)
    {
        _partyService = partyService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PartyDto>>> GetAll()
    {
        var parties = await _partyService.GetAllPartiesAsync();
        return Ok(parties);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PartyDto>> GetById(int id)
    {
        var party = await _partyService.GetPartyByIdAsync(id);
        return Ok(party);
    }

    [HttpPost]
    public async Task<ActionResult<PartyDto>> Create(CreatePartyDto dto)
    {
        var party = await _partyService.CreatePartyAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = party.Id }, party);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<PartyDto>> Update(int id, UpdatePartyDto dto)
    {
        var party = await _partyService.UpdatePartyAsync(id, dto);
        return Ok(party);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _partyService.DeletePartyAsync(id);
        return NoContent();
    }
}   