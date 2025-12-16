using AI_Marketplace.Application.Addresses.DTOs;
using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Domain.Entities;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Addresses.Commands
{
    public class UpdateAddressCommandHandler : IRequestHandler<UpdateAddressCommand, AddressResponseDto?>
    {
        private readonly IAddressRepository _repo;
        private readonly IMapper _mapper;

        public UpdateAddressCommandHandler(IAddressRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<AddressResponseDto?> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
        {
            var address = await _repo.GetAddressByIdAsync(request.AddressDto.Id);

            if (address == null)
                return null;

            if (address.UserId != request.UserId)
                throw new UnauthorizedAccessException("You cannot edit this address");

            address.Street = request.AddressDto.Street;
            address.City = request.AddressDto.City;
            address.State = request.AddressDto.State;
            address.PostalCode = request.AddressDto.PostalCode;
            address.Country = request.AddressDto.Country;
            address.IsPrimary = request.AddressDto.IsPrimary;

            await _repo.UpdateAddressAsync(address);

            return _mapper.Map<AddressResponseDto>(address);
        }
    }
}
