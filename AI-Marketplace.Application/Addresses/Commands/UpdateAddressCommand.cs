using AI_Marketplace.Application.Addresses.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Addresses.Commands
{
    public class UpdateAddressCommand : IRequest<AddressResponseDto>
    {
        public UpdateAddressRequestDto AddressDto { get; set; }
        public UpdateAddressCommand(UpdateAddressRequestDto addressDto)
        {
            AddressDto = addressDto;
        }
    }
    
   
}
