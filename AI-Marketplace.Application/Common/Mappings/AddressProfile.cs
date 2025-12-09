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
            CreateMap<Address, AddressResponseDto>();
            CreateMap<CreateAddressRequestDto, Address>();
            CreateMap<UpdateAddressRequestDto, Address>();
        }
    }
}
