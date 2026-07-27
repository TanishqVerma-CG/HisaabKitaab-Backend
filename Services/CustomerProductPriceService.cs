using AutoMapper;
using BillingSystem.Data.Repositories;
using BillingSystem.DTOs;
using BillingSystem.Models;

namespace BillingSystem.Services;

public class CustomerProductPriceService : ICustomerProductPriceService
{
    private readonly ICustomerProductPriceRepository _repository;
    private readonly IMapper _mapper;

    public CustomerProductPriceService(ICustomerProductPriceRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CustomerProductPriceDto>> GetPricesByCustomerAsync(int customerId)
    {
        var prices = await _repository.GetByCustomerIdAsync(customerId);
        return _mapper.Map<IEnumerable<CustomerProductPriceDto>>(prices);
    }

    public async Task<CustomerProductPriceDto> CreatePriceAsync(CreateCustomerProductPriceDto dto)
    {
        var existing = await _repository.GetByCustomerAndProductAsync(dto.CustomerId, dto.ProductId);
        if (existing != null)
            throw new Exception("Price already exists for this customer and product");

        var price = _mapper.Map<CustomerProductPrice>(dto);
        await _repository.AddAsync(price);

        var created = await _repository.GetByCustomerAndProductAsync(dto.CustomerId, dto.ProductId);
        return _mapper.Map<CustomerProductPriceDto>(created);
    }

    public async Task<CustomerProductPriceDto> UpdatePriceAsync(int id, UpdateCustomerProductPriceDto dto)
    {
        var price = await _repository.GetByIdAsync(id);
        if (price == null)
            throw new Exception("Price not found");

        price.Price = dto.Price;
        price.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(price);

        return _mapper.Map<CustomerProductPriceDto>(price);
    }

    public async Task DeletePriceAsync(int id)
    {
        var price = await _repository.GetByIdAsync(id);
        if (price == null)
            throw new Exception("Price not found");

        await _repository.DeleteAsync(price);
    }
}