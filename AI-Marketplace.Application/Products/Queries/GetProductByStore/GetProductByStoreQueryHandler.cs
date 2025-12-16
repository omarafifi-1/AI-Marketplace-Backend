using AI_Marketplace.Application.Common.Exceptions;
using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Products.DTOs;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AI_Marketplace.Application.Products.Queries.GetProductByStore
{
    public class GetProductByStoreQueryHandler : IRequestHandler<GetProductByStoreQuery, List<GetProductDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IMapper _mapper;

        public GetProductByStoreQueryHandler(IProductRepository productRepository, IMapper mapper, IStoreRepository storeRepository)
        {
            _productRepository = productRepository;
            _storeRepository = storeRepository;
            _mapper = mapper;

        }
        public async Task<List<GetProductDto>> Handle(GetProductByStoreQuery request, CancellationToken cancellationToken)
        {
            var store = await _storeRepository.GetByOwnerIdAsync(request.UserId, cancellationToken);
            if (store == null)
            {
                throw new NotFoundException(new Dictionary<string, string[]>
                {
                    { "Store", new[] { "Store not found for the given user." } }
                });
            }
            var products = await _productRepository.GetByStoreIdAsync(store.Id, cancellationToken);
            var productDtos = products.Select(product => _mapper.Map<GetProductDto>(product)).ToList();
            return productDtos;
        }

    }
}
