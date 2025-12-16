using AI_Marketplace.Application.Carts.DTOs;
using AI_Marketplace.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Carts.Commands
{
    public class UpdateCartQuantityCommandHandler : IRequestHandler<UpdateCartQuantityCommand, CartDto?>
    {
        private readonly ICartRepository _cartrepo;
        private readonly IMapper _mapper;

        public UpdateCartQuantityCommandHandler(ICartRepository cartrepo, IMapper mapper)
        {
            _cartrepo = cartrepo;
            _mapper = mapper;
        }
        public async Task<CartDto?> Handle(UpdateCartQuantityCommand request, CancellationToken cancellationToken)
        {
            var UserCart = await _cartrepo.GetCartByUserIdAsync(request.UserId, cancellationToken);
            if (UserCart == null)
                return null;
            var UserCartItem = await _cartrepo.GetCartItemByCartIdAndProductIdAsync(UserCart.Id, request.ProdId, cancellationToken);
            if (UserCartItem == null) 
                return null;
            UserCartItem.UpdatedAt = DateTime.UtcNow;
            UserCartItem.Quantity = request.Quantity;
            await _cartrepo.UpdateCartItemAsync(UserCartItem, cancellationToken);


            var UpdatedCart = await _cartrepo.GetCartByUserIdAsync(request.UserId, cancellationToken);
            return _mapper.Map<CartDto>(UpdatedCart);
        }
    }
}
