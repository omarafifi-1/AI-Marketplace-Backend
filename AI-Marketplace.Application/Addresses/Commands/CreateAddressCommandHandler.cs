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
    public class CreateAddressCommandHandler : IRequestHandler<CreateAddressCommand, CreateAddressDto>
    {
        private readonly IAddressRepository _repo;
        private readonly IMapper _mapper;

        public CreateAddressCommandHandler(IAddressRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        public async Task<CreateAddressDto> Handle(CreateAddressCommand request, CancellationToken cancellationToken)
        {
            var address = _mapper.Map<Address>(request.AddressDto);

            var result = await _repo.AddAddressAsync(address);

            return _mapper.Map<CreateAddressDto>(result);
        }
    }
}
