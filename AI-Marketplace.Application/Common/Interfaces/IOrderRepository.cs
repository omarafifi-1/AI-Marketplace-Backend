using AI_Marketplace.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Common.Interfaces
{
    public interface IOrderRepository
    {
        public Task<List<Order>> GetAllOrdersAsync(CancellationToken cancellationToken);
        public Task<Order?> GetOrderByIdAsync(int id, CancellationToken cancellationToken);
        public Task<List<Order>> GetOrdersByStoreIdAsync(int storeId, CancellationToken cancellationToken);
        public Task UpdateOrderAsync(Order order, CancellationToken cancellationToken);
        
    }
}
