using AI_Marketplace.Application.Common.Exceptions;
using AI_Marketplace.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Categories.Commands
{
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, string>
    {
        private readonly ICategoryRepository _categoryRepository;

        public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository) 
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<string> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetByIdAsync(request.Id);
            if (category == null)
            {
                throw new NotFoundException(new Dictionary<string, string[]>
                {
                    { "Category", new[] { $"Category With ID {request.Id} Not Found." } }
                });
            }
            await _categoryRepository.DeleteAsync(request.Id);
            return $"Category With ID {request.Id} Has Been Deleted Successfully.";
        }
    }
}
