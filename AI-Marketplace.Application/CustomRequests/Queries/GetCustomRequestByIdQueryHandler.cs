using AI_Marketplace.Application.Common.Exceptions;
using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.CustomRequests.DTOs;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.CustomRequests.Queries
{
    public class GetCustomRequestByIdQueryHandler : IRequestHandler<GetCustomRequestByIdQuery, CustomRequestResponseDto>
    {
        private readonly ICustomRequestRepository _customRequestRepository;
        private readonly IMapper _mapper;

        public GetCustomRequestByIdQueryHandler(ICustomRequestRepository customRequestRepository , IMapper mapper)
        {
            _customRequestRepository = customRequestRepository;
            _mapper = mapper;
        }
        public async Task<CustomRequestResponseDto?> Handle(GetCustomRequestByIdQuery request, CancellationToken cancellationToken)
        {
            var customRequest = await _customRequestRepository.GetByIdAsync(request.Id);

            if (customRequest == null)
            {
                throw new NotFoundException(new Dictionary<string, string[]>
                {
                    { "CustomRequest", new[] { $"Custom request with ID {request.Id} not found." } }
                });
            }

            // Sellers and Admins can view any request 
            // Customers can only view their own requests
            if (request.UserRole == "Customer" && customRequest.BuyerId != request.UserId)
            {
                throw new UnauthorizedAccessException( "You can only view your own custom requests.");
            }
            return _mapper.Map<CustomRequestResponseDto>(customRequest);
        }
    }
}
