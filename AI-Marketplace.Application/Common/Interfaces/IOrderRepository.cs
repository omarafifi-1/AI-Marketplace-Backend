using AI_Marketplace.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Common.Interfaces
{
    public interface IOrderRepository
    {
        public Task<Order> CreateAsync(Order order, CancellationToken cancellationToken);
        
        public Task<Order?> GetOrderByIdAsync(int orderId, CancellationToken cancellationToken);
        
        public Task<Order?> GetOrderByOfferIdAsync(int offerId, CancellationToken cancellationToken);
        
        public Task<List<Order>> GetAllOrdersAsync(CancellationToken cancellationToken);
        
        public Task<List<Order>> GetOrdersByStoreIdAsync(int storeId, CancellationToken cancellationToken);
        
        public Task UpdateOrderAsync(Order order, CancellationToken cancellationToken);
    }
}
