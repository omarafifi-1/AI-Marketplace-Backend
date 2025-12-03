using AI_Marketplace.Application.Offers.DTOs;
using AI_Marketplace.Application.Orders.DTOs;
using AI_Marketplace.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Common.Mappings
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<OrderItem, OrderItemDto>()
            .ForMember(d => d.ProductName, opt => opt.MapFrom(s => s.Product != null ? s.Product.Name : string.Empty))
            .ForMember(d => d.ImageUrl, opt => opt.MapFrom(s => s.Product != null && s.Product.ProductImages.Any()
                ? s.Product.ProductImages.OrderByDescending(pi => pi.IsPrimary).Select(pi => pi.ImageUrl).FirstOrDefault()
                : null));

            CreateMap<Order, OrderDto>()
           .ForMember(d => d.BuyerName, opt => opt.MapFrom(s => s.Buyer != null ? s.Buyer.UserName : string.Empty))
           .ForMember(d => d.StoreName, opt => opt.MapFrom(s => s.Store != null ? s.Store.StoreName : string.Empty))
           .ForMember(d => d.Items, opt => opt.MapFrom(s => s.OrderItems));

            CreateMap<Order, OrderResponseDto>()
                .ForMember(d => d.BuyerName, opt => opt.MapFrom(s => s.Buyer != null ? s.Buyer.UserName : string.Empty))
                .ForMember(d => d.StoreName, opt => opt.MapFrom(s => s.Store != null ? s.Store.StoreName : string.Empty))
                .ForMember(d => d.Offer, opt => opt.MapFrom(s => s.Offer));
        }
    }
}
