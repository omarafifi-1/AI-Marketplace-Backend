using AI_Marketplace.Application.Wishlists.DTOs;
using MediatR;

namespace AI_Marketplace.Application.Wishlists.Commands
{
    public record AddProductToWishlistCommand(int UserId, int ProductId) : IRequest<WishlistItemDto?>;
}
