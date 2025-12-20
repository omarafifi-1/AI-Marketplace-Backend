using AI_Marketplace.Application.Common.Exceptions;
using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Domain.Entities;
using AI_Marketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AI_Marketplace.Infrastructure.Repositories.Orders
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        public OrderRepository(ApplicationDbContext applicationDbContext) 
        {
            _context = applicationDbContext;
        }

        //CRUD operations for orders
        public async Task<Order> CreateAsync(Order order, CancellationToken cancellationToken = default)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync(cancellationToken);
            return order;
        }

        public Task<Order?> GetOrderByIdAsync(int orderId, CancellationToken cancellationToken = default)
        {
            return _context.Orders
                .Include(o => o.Buyer)
                .Include(o => o.Store)
                .Include(o => o.Offer)
                    .ThenInclude(of => of.Store)
                .Include(o => o.Offer)
                    .ThenInclude(of => of.CustomRequest)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
        }

        public Task<Order?> GetOrderByOfferIdAsync(int offerId, CancellationToken cancellationToken = default)
        {
            return _context.Orders
                .Include(o => o.Buyer)
                .Include(o => o.Store)
                .Include(o => o.Offer)
                    .ThenInclude(of => of.Store)
                .Include(o => o.Offer)
                    .ThenInclude(of => of.CustomRequest)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OfferId == offerId, cancellationToken);
        }

        public Task<List<Order>> GetAllOrdersAsync(CancellationToken cancellationToken = default)
        {
            return _context.Orders
                .Include(o => o.Buyer)
                .Include(o => o.Store)
                .Include(o => o.Offer)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ThenInclude (p => p.ProductImages)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Order>> GetOrdersByBuyerIdAsync(int buyerId, CancellationToken cancellationToken)
        {
            return await _context.Orders
                    .Where(o => o.BuyerId == buyerId)
                    .Include(o => o.Store)
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .ToListAsync(cancellationToken);
        }

        public async Task<List<Order>> GetOrdersByStoreIdAsync(int storeId, CancellationToken cancellationToken)
        {
            return await _context.Orders
                .Include(o => o.Buyer)
                .Include(o => o.Store)
                .Include(o => o.Offer)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.StoreId == storeId)
                .ToListAsync(cancellationToken);
        }

        public async Task UpdateOrderAsync(Order order, CancellationToken cancellationToken = default)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> CancelOrderByOrderIdAsync(int  orderId, CancellationToken cancellationToken)
        {
            return (await _context.Orders
                                  .Where(o => o.Id == orderId)
                                  .ExecuteDeleteAsync(cancellationToken)) > 0;
        }

        // CRUD operations for order items

        public async Task<OrderItem?> AddOrderItem(OrderItem orderItem, CancellationToken cancellationToken = default)
        {
            await _context.OrderItems.AddAsync(orderItem, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return orderItem;
        }

        public async Task<OrderItem?> GetOrderItemById(int id, CancellationToken cancellationToken = default)
        {
            return await _context.OrderItems
                .Include(oi => oi.Product)
                .Include(oi => oi.Order)
                .FirstOrDefaultAsync(oi => oi.Id == id, cancellationToken);
        }

        public async Task<List<OrderItem>> GetOrderItemsByOrderId(int orderId, CancellationToken cancellationToken = default)
        {
            return await _context.OrderItems
                .Include(oi => oi.Product)
                .Where(oi => oi.OrderId == orderId)
                .ToListAsync(cancellationToken);
        }

        public async Task UpdateOrderItem(OrderItem orderItem, CancellationToken cancellationToken = default)
        {
            _context.OrderItems.Update(orderItem);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> RemoveOrderItem(int orderItemId, CancellationToken cancellationToken = default)
        {
            var removed = await _context.OrderItems
                .Where(oi => oi.Id == orderItemId)
                .ExecuteDeleteAsync(cancellationToken);

            return removed > 0;
        }
    }
}
