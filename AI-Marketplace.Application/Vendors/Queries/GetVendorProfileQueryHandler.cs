using AI_Marketplace.Application.Common.Exceptions;
using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Vendors.DTOs;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace AI_Marketplace.Application.Vendors.Queries
{
    public class GetVendorProfileQueryHandler : IRequestHandler<GetVendorProfileQuery, VendorProfileDto>
    {
        private readonly IStoreRepository _storeRepository;
        private readonly IMapper _mapper;

        public GetVendorProfileQueryHandler(IStoreRepository storeRepository, IMapper mapper)
        {
            _storeRepository = storeRepository;
            _mapper = mapper;
        }

        public async Task<VendorProfileDto> Handle(GetVendorProfileQuery request, CancellationToken cancellationToken)
        {
            var store = await _storeRepository.GetByOwnerIdAsync(request.UserId, cancellationToken);
            if (store == null)
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Store", new[] { "Store not found for the given user." } }
                });
            }
            var vendorProfileDto = _mapper.Map<VendorProfileDto>(store);
            return vendorProfileDto;
        }
    }
}
