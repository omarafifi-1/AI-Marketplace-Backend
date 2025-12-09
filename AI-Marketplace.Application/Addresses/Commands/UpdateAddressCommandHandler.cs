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
            var address = _mapper.Map<Address>(request.AddressDto);

            var updated = await _repo.UpdateAddressAsync(address);

            return updated == null ? null : _mapper.Map<AddressResponseDto>(updated);
        }
    }
}
