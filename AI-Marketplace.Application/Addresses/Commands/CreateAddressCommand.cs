using AI_Marketplace.Application.Addresses.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Addresses.Commands
{
    public class CreateAddressCommand : IRequest<CreateAddressDto>
    {
        public CreateAddressRequestDto AddressDto { get; set; }
        public CreateAddressCommand(CreateAddressRequestDto addressDto)
        {
            AddressDto = addressDto;
        }

    }
}
