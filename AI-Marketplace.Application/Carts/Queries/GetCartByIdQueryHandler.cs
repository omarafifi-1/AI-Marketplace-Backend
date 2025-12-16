using AI_Marketplace.Application.Carts.DTOs;
using AI_Marketplace.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace AI_Marketplace.Application.Carts.Queries
{
    public class GetCartByIdQueryHandler : IRequestHandler<GetCartByIdQuery, CartDto?>
    {
        private readonly ICartRepository _CartRepo;
        private readonly IMapper _mapper;

        public GetCartByIdQueryHandler(ICartRepository CartRepo, IMapper mapper) {
            _CartRepo = CartRepo;
            _mapper = mapper;
        }

        async Task<CartDto?> IRequestHandler<GetCartByIdQuery, CartDto?>.Handle(GetCartByIdQuery request, CancellationToken cancellationToken)
        {
            var Cart = await _CartRepo.GetCartByCartIdAsync(request.Id, cancellationToken);
            if (Cart == null)
                return null;
            return _mapper.Map<CartDto>(Cart);
        }
    }
}
