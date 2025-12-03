using AI_Marketplace.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Common.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> CreateAsync(Order order, CancellationToken cancellationToken);
        
        Task<Order?> GetOrderByIdAsync(int orderId, CancellationToken cancellationToken);
        
        Task<Order?> GetByOfferIdAsync(int offerId, CancellationToken cancellationToken);
        
        Task<List<Order>> GetAllOrdersAsync(CancellationToken cancellationToken);
        
        Task<List<Order>> GetOrdersByStoreIdAsync(int storeId, CancellationToken cancellationToken);
        
        Task UpdateOrderAsync(Order order, CancellationToken cancellationToken);
    }
}
