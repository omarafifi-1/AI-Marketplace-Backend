using AI_Marketplace.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Common.Interfaces
{
    public interface ICartRepository
    {
        //CRUD operations for cart 
        Task<Cart> CreateNewCartAsync(Cart cart, CancellationToken cancellationToken);
        Task<Cart?> GetCartByCartIdAsync(int cartId, CancellationToken cancellationToken);
        Task<Cart?> GetCartByUserIdAsync(int userId, CancellationToken cancellationToken);
        Task<bool> UpdateCartAsync(Cart cart, CancellationToken cancellationToken);
        Task<bool> DeleteCartAsync(int cartId, CancellationToken cancellationToken);

        //CRUD operations for cart items
        Task<CartItem> AddItemToCartAsync(CartItem cartItem, CancellationToken cancellationToken);
        Task<CartItem?> GetCartItemByCartIdAndProductIdAsync(int cartId, int productId, CancellationToken cancellationToken);
        Task<CartItem?> GetCartItemByIdAsync(int cartItemId, CancellationToken cancellationToken);
        Task<bool> UpdateCartItemAsync(CartItem cartItem, CancellationToken cancellationToken);
        Task<bool> DeleteCartItemAsync(int cartItemId, CancellationToken cancellationToken);
        Task<IEnumerable<CartItem>> GetCartItemsByCartIdAsync(int cartId, CancellationToken cancellationToken);
        Task<bool> CartItemExistsAsync(int cartId, int productId, CancellationToken cancellationToken);
        Task<bool> ClearCartItemsAsync(int cartId, CancellationToken cancellationToken);

        //Database related relations
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
