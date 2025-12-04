using AI_Marketplace.Application.Users.DTOs;
using AI_Marketplace.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Common.Mappings
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<ApplicationUser, GetUserProfileDto>();

            CreateMap<UpdateUserProfileDto, ApplicationUser>()
          .ForMember(dest => dest.Id, opt => opt.Ignore())
          .ForMember(dest => dest.Email, opt => opt.Ignore())
          .ForMember(dest => dest.UserName, opt => opt.Ignore())
          .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
          .ForMember(dest => dest.IsActive, opt => opt.Ignore())
          .ForMember(dest => dest.LastModifiedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
        }
    }
}
