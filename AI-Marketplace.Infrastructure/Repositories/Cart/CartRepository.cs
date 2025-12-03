using AI_Marketplace.Application.Common.Exceptions;
using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Domain.Entities;
using AI_Marketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Infrastructure.Repositories.Cart
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public CartRepository(ApplicationDbContext applicationDbContext) {
            _applicationDbContext = applicationDbContext;
        }
        public async Task<CartItem> AddItemToCartAsync(CartItem cartItem, CancellationToken cancellationToken)
        {
            await _applicationDbContext.CartItems.AddAsync(cartItem, cancellationToken);
            await _applicationDbContext.SaveChangesAsync(cancellationToken);
            return cartItem;
        }

        //check if the cart item exists inside the cart and has the product with the given id.
        public async Task<bool> CartItemExistsAsync(int cartId, int productId, CancellationToken cancellationToken)
        {
            return await _applicationDbContext.CartItems.AnyAsync(ci => ci.CartId == cartId && ci.ProductId == productId, cancellationToken);
        }

        //Creates a new cart and saves it in the database
        public async Task<Domain.Entities.Cart> CreateNewCartAsync(Domain.Entities.Cart cart, CancellationToken cancellationToken)
        {
            await _applicationDbContext.Carts.AddAsync(cart, cancellationToken);
            await _applicationDbContext.SaveChangesAsync(cancellationToken);
            return cart;
        }

        public async Task<bool> DeleteCartAsync(int CartId, CancellationToken cancellationToken)
        {
            var AffectedRows =  await _applicationDbContext.Carts
                                        .Where(cart =>  cart.Id == CartId)
                                        .ExecuteDeleteAsync(cancellationToken);
            return AffectedRows > 0;
        }

        public async Task<bool> DeleteCartItemAsync(int cartItemId, CancellationToken cancellationToken)
        {
            var AffectedRows = await _applicationDbContext.CartItems.Where(ci => ci.Id ==  cartItemId).ExecuteDeleteAsync(cancellationToken);
            return AffectedRows > 0;
        }

        public async Task<Domain.Entities.Cart?> GetCartByCartIdAsync(int cartId, CancellationToken cancellationToken)
        {
            return await _applicationDbContext.Carts
                        .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Product)
                        .AsSplitQuery()
                        .FirstOrDefaultAsync(c => c.Id == cartId, cancellationToken);
        }

        public async Task<Domain.Entities.Cart?> GetCartByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            return await _applicationDbContext.Carts
                        .Where(c => c.UserId == userId)
                        .Include(cart => cart.CartItems)
                        .ThenInclude(CartItem => CartItem.Product)
                        .AsSplitQuery()
                        .SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<CartItem?> GetCartItemByCartIdAndProductIdAsync(int cartId, int productId, CancellationToken cancellationToken)
        {
            return await _applicationDbContext.CartItems
                                              .Include(ci => ci.Product)
                                              .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId, cancellationToken);
        }

        public async Task<CartItem?> GetCartItemByIdAsync(int cartItemId, CancellationToken cancellationToken)
        {
            return await _applicationDbContext.CartItems
                                              .AsNoTracking()
                                              .Include(ci => ci.Product)
                                              .SingleOrDefaultAsync(ci => ci.Id == cartItemId, cancellationToken);
        }

        public async Task<IEnumerable<CartItem>> GetCartItemsByCartIdAsync(int cartId, CancellationToken cancellationToken)
        {
            return await _applicationDbContext.CartItems
                                              .AsNoTracking()
                                              .Where(ci => ci.CartId == cartId)
                                              .Include(ci => ci.Product)
                                              .ToListAsync(cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _applicationDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> UpdateCartAsync(Domain.Entities.Cart cart, CancellationToken cancellationToken)
        {
            var record = await _applicationDbContext.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.Id == cart.Id, cancellationToken);
            if (record is null) 
                return false;
            record.UpdatedAt = DateTime.UtcNow;

            //convert the list of cartItem into a dictionary with id of the cartitem object as key and the value will be the cartItem object as a whole
            var ExistingItems =  record.CartItems.ToDictionary(i =>  i.Id);
            var EncomingItems = cart.CartItems;

            foreach (var item in EncomingItems)
            {
                if (ExistingItems.TryGetValue(item.Id, out CartItem? ExistingItem) && ExistingItem != null)
                {
                    ExistingItem.UpdatedAt = DateTime.UtcNow;
                    ExistingItem.Quantity = item.Quantity;
                }
                else
                {
                    record.CartItems.Add(item);
                }
            }
            var incomingIds = EncomingItems.Select(i => i.Id).ToHashSet();

            var itemsToRemove = record.CartItems
                .Where(i => !incomingIds.Contains(i.Id))
                .ToList(); 

            foreach (var item in itemsToRemove)
            {
                record.CartItems.Remove(item);
            }
            await _applicationDbContext.SaveChangesAsync(cancellationToken);
            return true;

        }

        public async Task<bool> UpdateCartItemAsync(CartItem cartItem, CancellationToken cancellationToken)
        {
            //make sure to update the nav property, cuz otherwise it will be detacted and any modifications made to it would result in possible 
            //issues like re-inserting into the database
           var record =  await _applicationDbContext.CartItems.SingleOrDefaultAsync(ci =>  ci.Id == cartItem.Id);
            if (record == null)
                return false;

            //would be sufficient enough to update the id and not the product nav property itself, it was necessary in the cartItems collection 
            //because it was a many to one relation with no FK 
           record.ProductId = cartItem.ProductId;
           record.Quantity = cartItem.Quantity;
           record.UpdatedAt = DateTime.UtcNow;
           await _applicationDbContext.SaveChangesAsync(cancellationToken);
           return true;
        }

        public async Task<bool> ClearCartItemsAsync(int cartId, CancellationToken cancellationToken)
        {
            var affectedRows = await _applicationDbContext.CartItems
                .Where(ci => ci.CartId == cartId)
                .ExecuteDeleteAsync(cancellationToken);
            
            var cart = await _applicationDbContext.Carts.FindAsync(new object[] { cartId }, cancellationToken);
            if (cart != null)
            {
                cart.UpdatedAt = DateTime.UtcNow;
                await _applicationDbContext.SaveChangesAsync(cancellationToken);
            }
            return affectedRows >= 0; 
        }
    }
}
