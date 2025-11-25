using AI_Marketplace.Application.Common.Exceptions;
using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Products.DTOs;
using AI_Marketplace.Domain.Entities;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace AI_Marketplace.Application.Products.Commands
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, GetProductDto>
    {
        private readonly IProductRepository _productRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CreateProductCommandHandler(IProductRepository productRepository, IStoreRepository storeRepository, ICategoryRepository categoryRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _storeRepository = storeRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }
        public async Task<GetProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var store = await this._storeRepository.GetByOwnerIdAsync(request.UserId, cancellationToken);
            if (store == null)
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Store", new[] { "Store Not Found For The Given User." } }
                });
            }

            var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);

            if (category == null)
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "CategoryId", new[] { "Category Not Found." } }
                });
            }
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Stock = request.Stock,
                CategoryId = request.CategoryId,
                IsActive = request.IsActive,
                StoreId = store.Id,
                CreatedAt = DateTime.UtcNow
            };
            await _productRepository.CreateAsync(product, cancellationToken);
            var createdProduct = await _productRepository.GetByIdAsync(product.Id, cancellationToken);
            var productDto = _mapper.Map<GetProductDto>(createdProduct);
            return productDto;
        }
            
    }
}   
