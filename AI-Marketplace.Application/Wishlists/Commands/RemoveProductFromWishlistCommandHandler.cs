using AI_Marketplace.Application.Common.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace AI_Marketplace.Application.Wishlists.Commands
{
    public class RemoveProductFromWishlistCommandHandler : IRequestHandler<RemoveProductFromWishlistCommand, bool>
    {
        private readonly IWishlistRepository _wishlistRepository;

        public RemoveProductFromWishlistCommandHandler(IWishlistRepository wishlistRepository)
        {
            _wishlistRepository = wishlistRepository;
        }

        public async Task<bool> Handle(RemoveProductFromWishlistCommand request, CancellationToken cancellationToken)
        {
            return await _wishlistRepository.RemoveFromWishlistAsync(request.UserId, request.ProductId, cancellationToken);
        }
    }
}
