using MediatR;

namespace AI_Marketplace.Application.Wishlists.Queries
{
    public record CheckProductInWishlistQuery(int UserId, int ProductId) : IRequest<bool>;
}
