using AI_Marketplace.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AI_Marketplace.Application.Common.Interfaces
{
    public interface IWishlistRepository
    {
        Task<Wishlist> AddToWishlistAsync(Wishlist wishlist, CancellationToken cancellationToken = default);
        Task<bool> RemoveFromWishlistAsync(int userId, int productId, CancellationToken cancellationToken = default);
        Task<List<Wishlist>> GetWishlistByUserIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<Wishlist?> GetWishlistItemAsync(int userId, int productId, CancellationToken cancellationToken = default);
        Task<bool> IsProductInWishlistAsync(int userId, int productId, CancellationToken cancellationToken = default);
    }
}
