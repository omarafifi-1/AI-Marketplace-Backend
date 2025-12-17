using AI_Marketplace.Application.CustomRequests.DTOs;
using AI_Marketplace.Domain.Entities;
using AI_Marketplace.Domain.enums;
using AutoMapper;

namespace AI_Marketplace.Application.Common.Mappings
{
    public class CustomRequestMappingProfile : Profile
    {
        public CustomRequestMappingProfile()
        {
            // Entity → Response DTO
            CreateMap<CustomRequest, CustomRequestResponseDto>()
                .ForMember(dest => dest.CategoryName,
                    opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
                .ForMember(dest => dest.BuyerName,
                    opt => opt.MapFrom(src => src.Buyer != null
                        ? (src.Buyer.FirstName + " " + src.Buyer.LastName).Trim()
                        : string.Empty))
                .ForMember(dest => dest.OffersCount,
                    opt => opt.MapFrom(src => src.Offers.Count))
                 .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString())); 

            
            CreateMap<CreateCustomRequestDto, CustomRequest>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.BuyerId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => CustomRequestStatus.Open))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Buyer, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.Offers, opt => opt.Ignore())
                .ForMember(dest => dest.GeneratedImages, opt => opt.Ignore());
        }
    }
}

