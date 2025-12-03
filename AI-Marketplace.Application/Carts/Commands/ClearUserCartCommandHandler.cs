using AI_Marketplace.Application.Carts.DTOs;
using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Domain.Entities;
using AutoMapper;
using MediatR;


namespace AI_Marketplace.Application.Carts.Commands
{
    public class ClearUserCartCommandHandler : IRequestHandler<ClearUserCartCommand, CartDto?>
    {
        private readonly ICartRepository _cartRepo;
        private readonly IMapper _mapper;

        public ClearUserCartCommandHandler(ICartRepository cartRepo , IMapper mapper)
        {
            _cartRepo = cartRepo;
            _mapper = mapper;
        }
        public async Task<CartDto?> Handle(ClearUserCartCommand request, CancellationToken cancellationToken)
        {
            var UserCart = await _cartRepo.GetCartByUserIdAsync(request.UserId, cancellationToken);
            if (UserCart == null)
                return null;

            await _cartRepo.ClearCartItemsAsync(UserCart.Id, cancellationToken);
            var clearedCart = await _cartRepo.GetCartByUserIdAsync(request.UserId, cancellationToken);
            return _mapper.Map<CartDto>(clearedCart);
        }
    }
}
