using AI_Marketplace.Application.Common.Interfaces;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AI_Marketplace.Application.Products.Commands
{
    public record SetPrimaryProductImageCommand : IRequest<bool>
    {
        public int ProductId { get; set; }
        public int ImageId { get; set; }
        public int UserId { get; set; }
    }

    public class SetPrimaryProductImageCommandHandler : IRequestHandler<SetPrimaryProductImageCommand, bool>
    {
        private readonly IProductRepository _productRepository;

        public SetPrimaryProductImageCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<bool> Handle(SetPrimaryProductImageCommand request, CancellationToken cancellationToken)
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

            // Set all images to non-primary
            foreach (var img in product.ProductImages)
            {
                img.IsPrimary = false;
            }

            // Set selected image to primary
            image.IsPrimary = true;

            await _productRepository.UpdateAsync(product, cancellationToken);

            return true;
        }
    }
}
