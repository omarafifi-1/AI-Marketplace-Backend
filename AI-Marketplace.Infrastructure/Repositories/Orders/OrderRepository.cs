using AI_Marketplace.Application.Common.Exceptions;
using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Domain.Entities;
using AI_Marketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Infrastructure.Repositories.Orders
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext applicationDbContext) 
        {
            _context = applicationDbContext;
        }

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
                .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
        }

        public Task<Order?> GetByOfferIdAsync(int offerId, CancellationToken cancellationToken = default)
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
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Order>> GetOrdersByStoreIdAsync(int storeId, CancellationToken cancellationToken = default)
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
    }
}
