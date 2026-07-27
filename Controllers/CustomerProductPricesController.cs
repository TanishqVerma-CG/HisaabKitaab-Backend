using Microsoft.AspNetCore.Mvc;
using BillingSystem.DTOs;
using BillingSystem.Services;

namespace BillingSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerProductPricesController : ControllerBase
{
    private readonly ICustomerProductPriceService _priceService;

    public CustomerProductPricesController(ICustomerProductPriceService priceService)
    {
        _priceService = priceService;
    }

    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<IEnumerable<CustomerProductPriceDto>>> GetByCustomer(int customerId)
    {
        var prices = await _priceService.GetPricesByCustomerAsync(customerId);
        return Ok(prices);
    }

    [HttpPost]
    public async Task<ActionResult<CustomerProductPriceDto>> Create(CreateCustomerProductPriceDto dto)
    {
        var price = await _priceService.CreatePriceAsync(dto);
        return Ok(price);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CustomerProductPriceDto>> Update(int id, UpdateCustomerProductPriceDto dto)
    {
        var price = await _priceService.UpdatePriceAsync(id, dto);
        return Ok(price);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _priceService.DeletePriceAsync(id);
        return NoContent();
    }
}