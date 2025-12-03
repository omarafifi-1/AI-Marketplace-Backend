using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using AI_Marketplace.Application.Carts.DTOs;
using AutoMapper;
using AI_Marketplace.Application.Common.Interfaces;

namespace AI_Marketplace.Application.Carts.Queries
{
    public class GetCartByUserIdQueryHandler: IRequestHandler<GetCartByUserIdQuery, CartDto?>
    {
        private readonly ICartRepository _cartRepo;
        private readonly IMapper _mapper;
        public GetCartByUserIdQueryHandler(ICartRepository cartRepo, IMapper mapper) {
            _cartRepo = cartRepo;
            _mapper = mapper;
        }
        async Task<CartDto?> IRequestHandler<GetCartByUserIdQuery, CartDto?>.Handle(GetCartByUserIdQuery request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepo.GetCartByUserIdAsync(request.UserId, cancellationToken);
            if (cart == null)
                return null;
            return _mapper.Map<CartDto>(cart);
        }
    }
}
