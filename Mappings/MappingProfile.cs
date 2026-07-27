using AutoMapper;
using BillingSystem.DTOs;
using BillingSystem.Models;

namespace BillingSystem.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Party, PartyDto>();
        CreateMap<CreatePartyDto, Party>();
        CreateMap<UpdatePartyDto, Party>();

        CreateMap<Product, ProductDto>();
        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>();

        CreateMap<CustomerProductPrice, CustomerProductPriceDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));
        CreateMap<CreateCustomerProductPriceDto, CustomerProductPrice>();

        CreateMap<Bill, BillDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
            .ForMember(dest => dest.CustomerGstNumber, opt => opt.MapFrom(src => src.Customer.GstNumber));

        CreateMap<BillItem, BillItemDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.HsnCode, opt => opt.MapFrom(src => src.Product.HsnCode));
    }
}