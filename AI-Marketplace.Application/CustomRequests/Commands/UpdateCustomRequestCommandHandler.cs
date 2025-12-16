using AI_Marketplace.Application.Common.Exceptions;
using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.CustomRequests.DTOs;
using AI_Marketplace.Domain.enums;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.CustomRequests.Commands
{
    public class UpdateCustomRequestCommandHandler : IRequestHandler<UpdateCustomRequestCommand, CustomRequestResponseDto>
    {
        private readonly ICustomRequestRepository _customRequestRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public UpdateCustomRequestCommandHandler(ICustomRequestRepository customRequestRepository ,
            ICategoryRepository categoryRepository , IMapper mapper)
        {
            _customRequestRepository = customRequestRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }
        public async Task<CustomRequestResponseDto> Handle(UpdateCustomRequestCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            var customRequest = _customRequestRepository.GetByIdAsync(request.Id, cancellationToken).Result;
            
            if(customRequest is null)
                throw new NotFoundException(new Dictionary<string, string[]>
                {
                    { "CustomRequest", new[] { $"Custom request with id {request.Id} not found." } }
                });

            if(customRequest.BuyerId != request.UserId)
                throw new UnauthorizedAccessException("You are not authorized to update this custom request.");

            // will make it work later after make the enum
            if (customRequest.Status == CustomRequestStatus.Completed)
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Status", new[] { "Cannot update a Completed custom request." } }
                });
            }

            if (request.CategoryId.HasValue && request.CategoryId.Value != customRequest.CategoryId)
            {
                var category = await _categoryRepository.GetByIdAsync(request.CategoryId.Value, cancellationToken);
                if (category == null)
                {
                    throw new ValidationException(new Dictionary<string, string[]>
                    {
                        { "Category", new[] { $"Category with ID {request.CategoryId.Value} not found." } }
                    });
                }
                customRequest.CategoryId = request.CategoryId.Value;
            }

            if (!string.IsNullOrWhiteSpace(request.Description))
                customRequest.Description = request.Description;


            if (request.ImageUrl != null) 
                customRequest.ImageUrl = request.ImageUrl;
            

            if (request.Budget.HasValue)
                customRequest.Budget = request.Budget.Value;


            if (request.Deadline.HasValue)
                 customRequest.Deadline = request.Deadline.Value;
     
            customRequest.UpdatedAt = DateTime.UtcNow;

            await _customRequestRepository.UpdateAsync(customRequest, cancellationToken);

            var updatedCustomRequest = await _customRequestRepository.GetByIdAsync(request.Id, cancellationToken);
            return _mapper.Map<CustomRequestResponseDto>(updatedCustomRequest);
        }
    }
}
