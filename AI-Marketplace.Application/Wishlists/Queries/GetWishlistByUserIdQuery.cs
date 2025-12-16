using AI_Marketplace.Application.Wishlists.DTOs;
using MediatR;

namespace AI_Marketplace.Application.Wishlists.Queries
{
    public record GetWishlistByUserIdQuery(int UserId) : IRequest<WishlistDto>;
}
