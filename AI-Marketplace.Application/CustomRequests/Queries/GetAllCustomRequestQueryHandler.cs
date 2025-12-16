using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.CustomRequests.DTOs;
using AI_Marketplace.Domain.enums;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.CustomRequests.Queries
{
    public class GetAllCustomRequestQueryHandler : IRequestHandler<GetAllCustomRequestQuery, List<CustomRequestResponseDto>>
    {
        private readonly ICustomRequestRepository _customRequestRepository;
        private readonly IMapper _mapper;

        public GetAllCustomRequestQueryHandler(ICustomRequestRepository customRequestRepository,IMapper mapper)
        {
            _customRequestRepository = customRequestRepository;
            _mapper = mapper;
        }
        public async Task<List<CustomRequestResponseDto>> Handle(GetAllCustomRequestQuery request, CancellationToken cancellationToken)
        {
            var customRequests = await _customRequestRepository.GetAllAsync(cancellationToken);

            // Apply filters
            var filtered = customRequests.AsQueryable();

            if (!string.IsNullOrEmpty(request.Status))
            {
                if (Enum.TryParse<CustomRequestStatus>(request.Status, true, out var statusEnum))
                {
                    filtered = filtered.Where(cr => cr.Status == statusEnum);
                }
            }
            if (request.CategoryId.HasValue)
            {
                filtered = filtered.Where(cr => cr.CategoryId == request.CategoryId.Value);
            }

            var result = _mapper.Map<List<CustomRequestResponseDto>>(filtered.ToList());

            return result;
        }
    }
}
