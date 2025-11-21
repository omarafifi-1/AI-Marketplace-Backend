using AI_Marketplace.Application.Products.DTOs;
using AI_Marketplace.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Common.Mappings
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, GetProductDto>()
                .ForMember(dest => dest.VendorName,
                opt => opt.MapFrom(src => src.Store.StoreName))
                .ForMember(dest => dest.ImageUrls,
                opt => opt.MapFrom(src => src.ProductImages.OrderByDescending(i => i.IsPrimary).Select(i => i.ImageUrl)));
        }
    }
}
