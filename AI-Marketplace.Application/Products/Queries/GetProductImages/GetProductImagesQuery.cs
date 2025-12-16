using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Products.DTOs;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AI_Marketplace.Application.Products.Queries.GetProductImages
{
    public record GetProductImagesQuery : IRequest<List<ProductImageDto>>
    {
        public int ProductId { get; set; }
    }

    public class GetProductImagesQueryHandler : IRequestHandler<GetProductImagesQuery, List<ProductImageDto>>
    {
        private readonly IProductRepository _productRepository;

        public GetProductImagesQueryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<List<ProductImageDto>> Handle(GetProductImagesQuery request, CancellationToken cancellationToken)
        {
            // Since IProductRepository.GetByIdAsync includes ProductImages, we can use that.
            // Or use GetQueryable if we want to project directly.
            
            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
            
            if (product == null)
            {
                 // Or return empty list? Usually if product doesn't exist, maybe 404 or empty.
                 // Let's return empty list or throw. Controller handles 404 for product usually.
                 // But here we are getting images.
                 // Let's return empty list if product not found or just null.
                 // The controller might want to know if product exists.
                 // For now, let's assume if product not found, no images.
                 return new List<ProductImageDto>();
            }

            return product.ProductImages
                .OrderByDescending(pi => pi.IsPrimary)
                .ThenByDescending(pi => pi.UploadedAt)
                .Select(pi => new ProductImageDto
                {
                    Id = pi.Id,
                    ProductId = pi.ProductId,
                    ImageUrl = pi.ImageUrl,
                    IsPrimary = pi.IsPrimary,
                    UploadedAt = pi.UploadedAt
                })
                .ToList();
        }
    }
}
