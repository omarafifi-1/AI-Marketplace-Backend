using AI_Marketplace.Application.Carts.DTOs;
using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Domain.Entities;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Carts.Commands
{

    public class AddProdToCartCommandHandler : IRequestHandler<AddProdToCartCommand, CartDto?>
    {
        private readonly ICartRepository _cartRepo;
        private readonly IProductRepository _productRepo;
        private readonly IMapper _mapper;
        
        public AddProdToCartCommandHandler(ICartRepository cartRepo, IProductRepository productRepo, IMapper mapper)
        {
            _cartRepo = cartRepo;
            _productRepo = productRepo;
            _mapper = mapper;
        }
        
        public async Task<CartDto?> Handle(AddProdToCartCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepo.GetByIdAsync(request.ProductId, cancellationToken);
            if (product == null || !product.IsActive)
            {
                return null;
            }

            var userCart = await _cartRepo.GetCartByUserIdAsync(request.ReqUserId, cancellationToken);
            
            if (userCart == null)
            {
                var newCart = new Cart
                {
                    UserId = request.ReqUserId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                
                userCart = await _cartRepo.CreateNewCartAsync(newCart, cancellationToken);
            }

            var existingCartItem = await _cartRepo.GetCartItemByCartIdAndProductIdAsync(userCart.Id, request.ProductId, cancellationToken);
            
            if (existingCartItem != null)
            {
                existingCartItem.Quantity += 1;
                existingCartItem.UpdatedAt = DateTime.UtcNow;
                await _cartRepo.UpdateCartItemAsync(existingCartItem, cancellationToken);
            }
            else
            {
                var newCartItem = new CartItem
                {
                    CartId = userCart.Id,
                    ProductId = request.ProductId,
                    Quantity = 1,
                    UnitPrice = product.Price,
                    AddedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                
                await _cartRepo.AddItemToCartAsync(newCartItem, cancellationToken);
            }
            var updatedCart = await _cartRepo.GetCartByUserIdAsync(request.ReqUserId, cancellationToken);
            return _mapper.Map<CartDto>(updatedCart);
        }
    }
}
