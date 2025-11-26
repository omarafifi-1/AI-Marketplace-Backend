using AI_Marketplace.Application.Categories.DTOs;
using AI_Marketplace.Application.Common.Exceptions;
using AI_Marketplace.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Categories.Queries
{
    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, GetCategoryDto>
    {
        private readonly ICategoryRepository _categoryRepository;

        public GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<GetCategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
            if (category == null)
            {
                throw new NotFoundException(new Dictionary<string, string[]>
                {
                    { "Category", new[] { "Category Not Found." } }
                });
            }
            return new GetCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ParentCategoryId = category.ParentCategoryId,
                CreatedAt = category.CreatedAt
            };
        }
    }
}
