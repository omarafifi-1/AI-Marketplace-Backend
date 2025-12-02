using AI_Marketplace.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Common.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> CreateAsync(Order order, CancellationToken cancellationToken = default);
        
        Task<Order?> GetByIdAsync(int orderId, CancellationToken cancellationToken = default);
        
        Task<Order?> GetByOfferIdAsync(int offerId, CancellationToken cancellationToken = default);
        
        Task<List<Order>> GetAllOrdersAsync(CancellationToken cancellationToken = default);
        
        Task<List<Order>> GetOrdersByStoreIdAsync(int storeId, CancellationToken cancellationToken = default);
        
        Task UpdateOrderAsync(Order order, CancellationToken cancellationToken = default);
    }
}
