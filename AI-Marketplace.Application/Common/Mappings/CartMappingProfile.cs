using AI_Marketplace.Application.Carts.DTOs;
using AI_Marketplace.Domain.Entities;
using AutoMapper;

namespace AI_Marketplace.Application.Common.Mappings
{
    public class CartMappingProfile : Profile
    {
        public CartMappingProfile()
        {
            CreateMap<Cart, CartDto>();

            // Map CartItem to CartItemDto
            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.UnitPrice * src.Quantity))
                .ForMember(dest => dest.ProductImageUrl,
                    opt => opt.MapFrom(src =>
                    src.Product.ProductImages
                    .Select(pi => pi.ImageUrl)
                    .FirstOrDefault()
                     ))
                .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.Product.Stock));
        }
    }
}