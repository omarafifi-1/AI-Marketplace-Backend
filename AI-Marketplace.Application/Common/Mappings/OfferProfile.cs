using System;
using System.Collections.Generic;
using System.Text;
using AI_Marketplace.Application.Offers.DTOs;
using AI_Marketplace.Domain.Entities;
using AutoMapper;

namespace AI_Marketplace.Application.Common.Mappings
{
    public class OfferProfile : Profile
    {
        public OfferProfile()
        {
            CreateMap<Offer, OfferResponseDto>()
                .ForMember(dest => dest.StoreName,
                    opt => opt.MapFrom(src => src.Store != null ? src.Store.StoreName : string.Empty));

            CreateMap<CreateOfferDto, Offer>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.StoreId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Pending"))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.CustomRequest, opt => opt.Ignore())
                .ForMember(dest => dest.Store, opt => opt.Ignore())
                .ForMember(dest => dest.Order, opt => opt.Ignore());
        }
    }
}
