using MediatR;

namespace AI_Marketplace.Application.Wishlists.Commands
{
    public record RemoveProductFromWishlistCommand(int UserId, int ProductId) : IRequest<bool>;
}
