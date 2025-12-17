using AI_Marketplace.Application.Addresses.DTOs;
using AI_Marketplace.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Common.Mappings
{
    public class AddressProfile : Profile
    {
        public AddressProfile()
        {
            CreateMap<Address, AddressResponseDto>().ReverseMap();
            CreateMap<Address, CreateAddressDto>().ReverseMap();
            CreateMap<CreateAddressRequestDto, Address>().ReverseMap()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));
            CreateMap<UpdateAddressRequestDto, Address>().ReverseMap();
            CreateMap<Address, CreateAddressDto>().ReverseMap();
        }
    }
}
