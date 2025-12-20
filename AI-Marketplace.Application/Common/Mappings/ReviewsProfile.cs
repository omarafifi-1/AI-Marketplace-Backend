using AI_Marketplace.Application.Reviews.DTOs;
using AI_Marketplace.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Common.Mappings
{
    public class ReviewsProfile : Profile 
    {
        public ReviewsProfile() {



            
            CreateMap<ProductReviewDto, Review>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.UserId, opt => opt.Ignore())
                .ForMember(d => d.StoreId, opt => opt.Ignore())
                .ForMember(d => d.CreatedAt, opt => opt.Ignore());

            
            CreateMap<Review, ReviewDto>()
                .ForMember(d => d.UserName,
                           opt => opt.MapFrom(s => s.User.UserName));



    }
    }
}
