using AI_Marketplace.Application.Common.Interfaces;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AI_Marketplace.Application.Products.Commands
{
    public record DeleteProductImageCommand : IRequest<bool>
    {
        public int ProductId { get; set; }
        public int ImageId { get; set; }
        public int UserId { get; set; }
    }

    public class DeleteProductImageCommandHandler : IRequestHandler<DeleteProductImageCommand, bool>
    {
        private readonly IProductRepository _productRepository;
        private readonly IFileService _fileService;

        public DeleteProductImageCommandHandler(IProductRepository productRepository, IFileService fileService)
        {
            _productRepository = productRepository;
            _fileService = fileService;
        }

        public async Task<bool> Handle(DeleteProductImageCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);

            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {request.ProductId} not found.");
            }

            if (product.Store.OwnerId != request.UserId)
            {
                throw new UnauthorizedAccessException("You are not authorized to modify this product.");
            }

            var image = product.ProductImages.FirstOrDefault(pi => pi.Id == request.ImageId);
            if (image == null)
            {
                throw new KeyNotFoundException($"Image with ID {request.ImageId} not found for this product.");
            }

            // Delete file from storage
            await _fileService.DeleteFileAsync(image.ImageUrl);

            // Remove from database
            await _productRepository.DeleteProductImageAsync(image, cancellationToken);

            // If deleted image was primary, set another one as primary if exists
            // Note: We need to re-fetch or check if the list in memory is updated.
            // Since we removed it via repository, the in-memory 'product.ProductImages' might still contain it depending on tracking.
            // But usually EF Core fixes up navigation.
            // However, to be safe, let's check remaining images.
            
            // Actually, if we just called DeleteProductImageAsync, the context tracks the deletion.
            // But we might need to update the product if we want to set a new primary.
            // Let's check if there are other images.
            
            if (image.IsPrimary && product.ProductImages.Count > 1)
            {
                 // We need to find another image to set as primary.
                 // Since 'image' is being deleted, we should pick another one.
                 // We can't rely on product.ProductImages excluding the deleted one immediately without reload.
                 // But we can filter it out.
                 var newPrimary = product.ProductImages.FirstOrDefault(pi => pi.Id != request.ImageId);
                 if (newPrimary != null)
                 {
                     newPrimary.IsPrimary = true;
                     await _productRepository.UpdateAsync(product, cancellationToken);
                 }
            }

            return true;
        }
    }
}
