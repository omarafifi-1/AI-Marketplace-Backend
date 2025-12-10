using AI_Marketplace.Application.Wishlists.DTOs;
using AI_Marketplace.Domain.Entities;
using AutoMapper;
using System.Linq;

namespace AI_Marketplace.Application.Common.Mappings
{
    public class WishlistProfile : Profile
    {
        public WishlistProfile()
        {
            CreateMap<Domain.Entities.Wishlist, WishlistItemDto>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Product.Id))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductDescription, opt => opt.MapFrom(src => src.Product.Description))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Product.IsActive))
                .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.Product.Stock))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Product.Rating))
                .ForMember(dest => dest.PrimaryImageUrl, opt => opt.MapFrom(src => 
                    src.Product.ProductImages.FirstOrDefault(img => img.IsPrimary) != null 
                    ? src.Product.ProductImages.FirstOrDefault(img => img.IsPrimary)!.ImageUrl 
                    : src.Product.ProductImages.FirstOrDefault() != null 
                        ? src.Product.ProductImages.FirstOrDefault()!.ImageUrl 
                        : null))
                .ForMember(dest => dest.StoreId, opt => opt.MapFrom(src => src.Product.Store.Id))
                .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Product.Store.StoreName))
                .ForMember(dest => dest.AddedOn, opt => opt.MapFrom(src => src.AddedOn));
        }
    }
}
