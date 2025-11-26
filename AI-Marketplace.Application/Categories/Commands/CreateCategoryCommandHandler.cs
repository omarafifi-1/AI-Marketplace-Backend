using AI_Marketplace.Application.Categories.DTOs;
using AI_Marketplace.Application.Common.Exceptions;
using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Domain.Entities;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Categories.Commands
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, GetCategoryDto>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IMapper mapper) 
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<GetCategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            if (request.ParentCategoryId.HasValue)
            {
                var parentCategory = await _categoryRepository.GetByIdAsync(request.ParentCategoryId.Value, cancellationToken);
                if (parentCategory == null)
                {
                    throw new NotFoundException(new Dictionary<string, string[]>
                    {
                        { "ParentCategory", new[] { $"Parent Category With ID {request.ParentCategoryId.Value} Not Found." } }
                    });
                }
            }

            var category = new Category
            {
                Name = request.Name,
                Description = request.Description,
                ParentCategoryId = request.ParentCategoryId,
                CreatedAt = DateTime.UtcNow
            };

            var createdCategory = await _categoryRepository.CreateAsync(category, cancellationToken);
            return _mapper.Map<GetCategoryDto>(createdCategory);
        }
    }
}
