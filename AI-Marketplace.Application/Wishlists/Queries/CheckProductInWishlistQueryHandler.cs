using AI_Marketplace.Application.Common.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace AI_Marketplace.Application.Wishlists.Queries
{
    public class CheckProductInWishlistQueryHandler : IRequestHandler<CheckProductInWishlistQuery, bool>
    {
        private readonly IWishlistRepository _wishlistRepository;

        public CheckProductInWishlistQueryHandler(IWishlistRepository wishlistRepository)
        {
            _wishlistRepository = wishlistRepository;
        }

        public async Task<bool> Handle(CheckProductInWishlistQuery request, CancellationToken cancellationToken)
        {
            return await _wishlistRepository.IsProductInWishlistAsync(request.UserId, request.ProductId, cancellationToken);
        }
    }
}
