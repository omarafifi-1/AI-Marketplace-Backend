using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Products.DTOs;
using AI_Marketplace.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AI_Marketplace.Application.Products.Commands
{
    public record UploadProductImageCommand : IRequest<ProductImageDto>
    {
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public IFormFile File { get; set; } = null!;
    }

    public class UploadProductImageCommandHandler : IRequestHandler<UploadProductImageCommand, ProductImageDto>
    {
        private readonly IProductRepository _productRepository;
        private readonly IFileService _fileService;

        public UploadProductImageCommandHandler(IProductRepository productRepository, IFileService fileService)
        {
            _productRepository = productRepository;
            _fileService = fileService;
        }

        public async Task<ProductImageDto> Handle(UploadProductImageCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);

            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {request.ProductId} not found.");
            }

            // Verify ownership
            if (product.Store.OwnerId != request.UserId)
            {
                 throw new UnauthorizedAccessException("You are not authorized to add images to this product.");
            }

            string imageUrl = await _fileService.SaveFileAsync(request.File, "images/products");

            var productImage = new ProductImage
            {
                ProductId = request.ProductId,
                ImageUrl = imageUrl,
                IsPrimary = product.ProductImages.Count == 0, // First image is primary
                UploadedAt = DateTime.UtcNow
            };

            await _productRepository.UploadProductImageAsync(productImage, cancellationToken);

            return new ProductImageDto
            {
                Id = productImage.Id,
                ProductId = productImage.ProductId,
                ImageUrl = productImage.ImageUrl,
                IsPrimary = productImage.IsPrimary,
                UploadedAt = productImage.UploadedAt
            };
        }
    }
}
