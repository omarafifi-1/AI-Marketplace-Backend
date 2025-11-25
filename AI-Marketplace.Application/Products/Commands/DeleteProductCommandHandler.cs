using AI_Marketplace.Application.Common.Exceptions;
using AI_Marketplace.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Products.Commands
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, string>
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;

        public DeleteProductCommandHandler(IMapper mapper, IProductRepository productRepository) 
        {
            _mapper = mapper;
            _productRepository = productRepository;
        }

        public async Task<string> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
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
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Store", new[] { "Associated Store Not Found." } }
                });
            }
            if (product.Store.OwnerId != request.UserId)
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Authorization", new[] { "You Are Not Authorized to Delete This Product." } }
                });
            }
            await _productRepository.DeleteAsync(product, cancellationToken);
            return "Product Deleted Successfully.";
        }
    }
}
