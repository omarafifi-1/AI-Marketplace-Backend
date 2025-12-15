using AI_Marketplace.Application.Addresses.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Addresses.Commands
{
    public class UpdateAddressCommand : IRequest<AddressResponseDto?>
    {
        public int UserId { get; set; }
        public UpdateAddressRequestDto AddressDto { get; set; }
        public UpdateAddressCommand(int userId, UpdateAddressRequestDto addressDto)
        {
            UserId = userId;
            AddressDto = addressDto;
        }
    }
    
   
}
