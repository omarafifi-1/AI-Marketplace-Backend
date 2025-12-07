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
        public Task<List<Order>> GetOrdersByBuyerIdAsync(int buyerId, CancellationToken cancellationToken);
        public Task<List<Order>> GetOrdersByStoreIdAsync(int storeId, CancellationToken cancellationToken);
        
        public Task UpdateOrderAsync(Order order, CancellationToken cancellationToken);
        public Task<bool> CancelOrderByOrderIdAsync(int orderId, CancellationToken cancellationToken);

        public Task<OrderItem?> AddOrderItem(OrderItem orderItem, CancellationToken cancellationToken);
        public Task<OrderItem?> GetOrderItemById(int id, CancellationToken cancellationToken);
        public Task<List<OrderItem>> GetOrderItemsByOrderId(int orderId, CancellationToken cancellationToken);
        public Task UpdateOrderItem(OrderItem orderItem, CancellationToken cancellationToken);
        public Task<bool> RemoveOrderItem(int orderItemId, CancellationToken cancellationToken);
    }
}
