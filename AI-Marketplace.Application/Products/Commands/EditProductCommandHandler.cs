using AI_Marketplace.Application.Common.Exceptions;
using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Products.DTOs;
using AI_Marketplace.Domain.Entities;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Products.Commands
{
    public class EditProductCommandHandler : IRequestHandler<EditProductCommand, GetProductDto>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public EditProductCommandHandler(ICategoryRepository categoryRepository, IProductRepository productRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }
        public async Task<GetProductDto> Handle(EditProductCommand request, CancellationToken cancellationToken)
        {

            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
            if (product == null)
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Product", new[] { "Product Not Found." } }
                });
            }

            if(product.Store == null)
            {
                throw new InvalidOperationException("Product Store Not Loaded.");
            }
            if (product.Store.OwnerId != request.UserId)
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Authorization", new[] { "User is Not The Owner of The Store." } }
                });
            }

            if(request.CategoryId != product.CategoryId)
            {
                var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);

                if (category == null)
                {
                    throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Category", new[] { "Category Not Found." } }
                });
                }
            }
            
            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.Stock = request.Stock;
            product.CategoryId = request.CategoryId;
            product.IsActive = request.IsActive;
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product, cancellationToken);
            var UpdatedProduct = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
            return _mapper.Map<GetProductDto>(UpdatedProduct);
        }
    }
}
