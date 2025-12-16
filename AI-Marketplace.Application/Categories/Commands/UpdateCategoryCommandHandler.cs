using AI_Marketplace.Application.Categories.DTOs;
using AI_Marketplace.Application.Common.Exceptions;
using AI_Marketplace.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Categories.Commands
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, GetCategoryDto>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }
        public async Task<GetCategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetByIdAsync(request.Id);
            if (category == null)
            {
                throw new NotFoundException (new Dictionary<string, string[]>
                {
                    { "Category", new[] { $"Category With Id {request.Id} Was Not Found." } }
                });
            }
            if (!string.IsNullOrEmpty(request.Name))
            {
                category.Name = request.Name;
            }
            if (request.Description != null)
            {
                category.Description = request.Description;
            }
            if (request.RemoveParentCategory)
            {
                category.ParentCategoryId = null;
            }

            else if(request.ParentCategoryId.HasValue)
            {
                var parentCategory = await _categoryRepository.GetByIdAsync(request.ParentCategoryId.Value);
                if (parentCategory == null)
                {
                    throw new NotFoundException(new Dictionary<string, string[]>
                    {
                        { "ParentCategory", new[] { $"Parent Category With Id {request.ParentCategoryId.Value} Was Not Found." } }
                    });
                }
                if (parentCategory.Id == category.Id)
                {
                    throw new ValidationException(new Dictionary<string, string[]>
                    {
                        { "ParentCategory", new[] { "A Category Cannot Be Its Own Parent." } }
                    });
                }
                category.ParentCategoryId = request.ParentCategoryId;
            }
            await _categoryRepository.UpdateAsync(category);
            return _mapper.Map<GetCategoryDto>(category);
        }
}
    }
