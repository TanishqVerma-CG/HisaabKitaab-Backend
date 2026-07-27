using AutoMapper;
using BillingSystem.Data.Repositories;
using BillingSystem.DTOs;
using BillingSystem.Models;

namespace BillingSystem.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;

    public ProductService(IProductRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<ProductDto> GetProductByIdAsync(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null)
            throw new Exception("Product not found");

        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
    {
        var product = _mapper.Map<Product>(dto);
        await _repository.AddAsync(product);
        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto dto)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null)
            throw new Exception("Product not found");

        _mapper.Map(dto, product);
        product.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(product);

        return _mapper.Map<ProductDto>(product);
    }

    public async Task DeleteProductAsync(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null)
            throw new Exception("Product not found");

        await _repository.DeleteAsync(product);
    }
}