using AI_Marketplace.Application.Categories.DTOs;
using AI_Marketplace.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Common.Mappings
{
    public class CategoryMapping : Profile
    {
        public CategoryMapping()
        {
            CreateMap<Category,GetCategoryDto>().ReverseMap();
        }
    }
}
