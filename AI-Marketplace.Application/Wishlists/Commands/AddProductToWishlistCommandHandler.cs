using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Wishlists.DTOs;
using AutoMapper;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AI_Marketplace.Application.Wishlists.Commands
{
    public class AddProductToWishlistCommandHandler : IRequestHandler<AddProductToWishlistCommand, WishlistItemDto?>
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public AddProductToWishlistCommandHandler(
            IWishlistRepository wishlistRepository,
            IProductRepository productRepository,
            IMapper mapper)
        {
            _wishlistRepository = wishlistRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<WishlistItemDto?> Handle(AddProductToWishlistCommand request, CancellationToken cancellationToken)
        {
            // Check if product exists and is active
            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
            if (product == null || !product.IsActive)
            {
                return null;
            }

            // Check if already in wishlist
            var existingItem = await _wishlistRepository.IsProductInWishlistAsync(request.UserId, request.ProductId, cancellationToken);
            if (existingItem)
            {
                // Already in wishlist, return existing item
                var existing = await _wishlistRepository.GetWishlistItemAsync(request.UserId, request.ProductId, cancellationToken);
                return _mapper.Map<WishlistItemDto>(existing);
            }

            // Add to wishlist
            var wishlistItem = new Domain.Entities.Wishlist
            {
                UserId = request.UserId,
                ProductId = request.ProductId,
                AddedOn = DateTime.UtcNow
            };

            var addedItem = await _wishlistRepository.AddToWishlistAsync(wishlistItem, cancellationToken);
            
            // Fetch the item with product details for mapping
            var itemWithDetails = await _wishlistRepository.GetWishlistItemAsync(request.UserId, request.ProductId, cancellationToken);
            
            return _mapper.Map<WishlistItemDto>(itemWithDetails);
        }
    }
}
