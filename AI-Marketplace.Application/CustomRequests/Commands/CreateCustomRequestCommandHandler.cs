using AI_Marketplace.Application.Common.Exceptions;
using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.CustomRequests.DTOs;
using AI_Marketplace.Domain.Entities;
using AI_Marketplace.Domain.enums;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.CustomRequests.Commands
{
    public class CreateCustomRequestCommandHandler : IRequestHandler<CreateCustomRequestCommand, CustomRequestResponseDto>
    {
        private readonly ICustomRequestRepository _customRequestRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CreateCustomRequestCommandHandler(
            ICustomRequestRepository customRequestRepository,
            ICategoryRepository categoryRepository,
            IMapper mapper)
        {
            _customRequestRepository = customRequestRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<CustomRequestResponseDto> Handle(
            CreateCustomRequestCommand request,
            CancellationToken cancellationToken)
        {
            
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            
            var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
            if (category == null)
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Category", new[] { "Category not found." } }
                });
            }

            
            var customRequest = new CustomRequest
            {
                Description = request.Description,
                CategoryId = request.CategoryId,
                BuyerId = request.UserId,  
                Status = CustomRequestStatus.Open,
                CreatedAt = DateTime.UtcNow,

                ImageUrl = request.ImageUrl,
                Budget = request.Budget,
                Deadline = request.Deadline,
            };

            
            var createdCustomRequest = await _customRequestRepository.CreateAsync(customRequest, cancellationToken);

            // Fetch full details with includes 
            var fullCustomRequest = await _customRequestRepository.GetByIdAsync(
                createdCustomRequest.Id,
                cancellationToken);

            
            return _mapper.Map<CustomRequestResponseDto>(fullCustomRequest);

        }
    }
}
