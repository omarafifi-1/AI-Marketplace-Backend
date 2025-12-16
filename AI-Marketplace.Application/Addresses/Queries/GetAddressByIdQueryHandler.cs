using AI_Marketplace.Application.Addresses.DTOs;
using AI_Marketplace.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Addresses.Queries
{
    public class GetAddressByIdQueryHandler : IRequestHandler<GetAddressByIdQuery, CreateAddressDto?>
    {
        private readonly IAddressRepository _repo;
        private readonly IMapper _mapper;

        public GetAddressByIdQueryHandler(IAddressRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<CreateAddressDto?> Handle(GetAddressByIdQuery request, CancellationToken cancellationToken)
        {
            var address = await _repo.GetAddressByIdAsync(request.Id);

            return address == null ? null : _mapper.Map<CreateAddressDto>(address);
        }
    }
}
