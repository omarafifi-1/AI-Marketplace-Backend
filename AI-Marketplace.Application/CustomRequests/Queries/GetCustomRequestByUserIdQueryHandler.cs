using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.CustomRequests.DTOs;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.CustomRequests.Queries
{
    public class GetCustomRequestByUserIdQueryHandler : IRequestHandler<GetCustomRequestByUserIdQuery, List<CustomRequestResponseDto>>
    {
        private readonly ICustomRequestRepository _customRequestRepository;
        private readonly IMapper _mapper;

        public GetCustomRequestByUserIdQueryHandler(ICustomRequestRepository customRequestRepository , IMapper mapper)
        {
            _customRequestRepository = customRequestRepository;
            _mapper = mapper;
        }
        public async Task<List<CustomRequestResponseDto>> Handle(GetCustomRequestByUserIdQuery request, CancellationToken cancellationToken)
        {
            var customRequests = await _customRequestRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            return _mapper.Map<List<CustomRequestResponseDto>>(customRequests);
        }
    }
}
