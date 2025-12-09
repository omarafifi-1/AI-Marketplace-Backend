using AI_Marketplace.Application.Addresses.DTOs;
using AI_Marketplace.Application.Common.Interfaces;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Addresses.Queries
{
    public class GetAddressesByUserIdQueryHandler
    {
        private readonly IAddressRepository _repo;
        private readonly IMapper _mapper;

        public GetAddressesByUserIdQueryHandler(IAddressRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AddressResponseDto>> Handle(GetAddressesByUserIdQuery request, CancellationToken cancellationToken)
        {
            var addresses = await _repo.GetAddressesByUserIdAsync(request.UserId);

            return _mapper.Map<IEnumerable<AddressResponseDto>>(addresses);
        }
    }
}
