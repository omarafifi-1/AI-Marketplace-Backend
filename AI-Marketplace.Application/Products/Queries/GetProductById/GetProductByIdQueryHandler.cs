using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Products.DTOs;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Products.Queries.GetProductById
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, GetProductDto?>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public GetProductByIdQueryHandler(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            
        }
        public async Task<GetProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.Id);

            if (product == null || !product.IsActive)
            {
                return null;
            }

            return _mapper.Map<GetProductDto>(product);
        }
    }
}
