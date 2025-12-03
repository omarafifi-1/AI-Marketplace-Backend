using AI_Marketplace.Application.Carts.DTOs;
using AI_Marketplace.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Carts.Commands
{
    public class RemoveProdFromCartCommandHandler : IRequestHandler<RemoveProdFromCartCommand, CartDto?>
    {
        private readonly ICartRepository _cartRepo;
        private readonly IMapper _mapper;

        public RemoveProdFromCartCommandHandler(ICartRepository cartRepo, IMapper mapper)
        {
            _cartRepo = cartRepo;
            _mapper = mapper;
        }
        
        public async Task<CartDto?> Handle(RemoveProdFromCartCommand request, CancellationToken cancellationToken)
        {   
            var userCart = await _cartRepo.GetCartByUserIdAsync(request.UserId, cancellationToken);
            if (userCart == null) 
                return null;

            var itemToBeDeleted = await _cartRepo.GetCartItemByCartIdAndProductIdAsync(userCart.Id, request.ProdId, cancellationToken);
            if (itemToBeDeleted == null) 
                return null;
            
            var result = await _cartRepo.DeleteCartItemAsync(itemToBeDeleted.Id, cancellationToken);
            if (!result) 
                return null;

            var updatedCart = await _cartRepo.GetCartByUserIdAsync(request.UserId, cancellationToken);
            return _mapper.Map<CartDto>(updatedCart);
        }
    }
}
