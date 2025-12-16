using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Domain.Entities;
using AI_Marketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AI_Marketplace.Infrastructure.Repositories.Wishlist
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly ApplicationDbContext _context;

        public WishlistRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Domain.Entities.Wishlist> AddToWishlistAsync(Domain.Entities.Wishlist wishlist, CancellationToken cancellationToken = default)
        {
            await _context.Wishlists.AddAsync(wishlist, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return wishlist;
        }

        public async Task<bool> RemoveFromWishlistAsync(int userId, int productId, CancellationToken cancellationToken = default)
        {
            var affectedRows = await _context.Wishlists
                .Where(w => w.UserId == userId && w.ProductId == productId)
                .ExecuteDeleteAsync(cancellationToken);
            
            return affectedRows > 0;
        }

        public async Task<List<Domain.Entities.Wishlist>> GetWishlistByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.Wishlists
                .Where(w => w.UserId == userId)
                .Include(w => w.Product)
                    .ThenInclude(p => p.ProductImages)
                .Include(w => w.Product)
                    .ThenInclude(p => p.Store)
                .OrderByDescending(w => w.AddedOn)
                .ToListAsync(cancellationToken);
        }

        public async Task<Domain.Entities.Wishlist?> GetWishlistItemAsync(int userId, int productId, CancellationToken cancellationToken = default)
        {
            return await _context.Wishlists
                .Include(w => w.Product)
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId, cancellationToken);
        }

        public async Task<bool> IsProductInWishlistAsync(int userId, int productId, CancellationToken cancellationToken = default)
        {
            return await _context.Wishlists
                .AnyAsync(w => w.UserId == userId && w.ProductId == productId, cancellationToken);
        }
    }
}
