using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Vendors.DTOs;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Admin.Queries
{
    public class GetAllVendorsQueryHandler : IRequestHandler<GetAllVendorsQuery, List<VendorProfileDto>>
    {
        private readonly IStoreRepository _storeRepository;
        private readonly IMapper _mapper;

        public GetAllVendorsQueryHandler(IStoreRepository storeRepository, IMapper mapper)
        {
            _storeRepository = storeRepository;
            _mapper = mapper;
        }
        public async Task<List<VendorProfileDto>> Handle(GetAllVendorsQuery request, CancellationToken cancellationToken)
        {
            var stores = await _storeRepository.GetAllStoresAsync();
            var vendorDtos = _mapper.Map<List<VendorProfileDto>>(stores);
            return vendorDtos;
        }
    }
}
