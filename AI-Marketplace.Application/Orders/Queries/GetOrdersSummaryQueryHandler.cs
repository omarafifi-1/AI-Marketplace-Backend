using AI_Marketplace.Application.Common.Interfaces;
using MediatR;

namespace AI_Marketplace.Application.Orders.Queries
{
    public class GetOrdersSummaryQueryHandler : IRequestHandler<GetOrdersSummaryQuery, OrdersSummaryDto>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IStoreRepository _storeRepository;

        public GetOrdersSummaryQueryHandler(IOrderRepository orderRepository, IStoreRepository storeRepository)
        {
            _orderRepository = orderRepository;
            _storeRepository = storeRepository;
        }

        public async Task<OrdersSummaryDto> Handle(GetOrdersSummaryQuery request, CancellationToken cancellationToken)
        {
            List<Domain.Entities.Order> orders;

            if (request.UserRole == "Admin")
            {
                orders = await _orderRepository.GetAllOrdersAsync(cancellationToken);
            }
            else if (request.UserRole == "Seller")
            {
                var store = await _storeRepository.GetByOwnerIdAsync(request.UserId, cancellationToken);
                if (store != null)
                {
                    orders = await _orderRepository.GetOrdersByStoreIdAsync(store.Id, cancellationToken);
                }
                else
                {
                    orders = new List<Domain.Entities.Order>();
                }
            }
            else
            {
                orders = await _orderRepository.GetOrdersByBuyerIdAsync(request.UserId, cancellationToken);
            }

            var summary = new OrdersSummaryDto
            {
                TotalOrders = orders.Count,
                PendingOrders = orders.Count(o => o.Status == "Pending"),
                ProcessingOrders = orders.Count(o => o.Status == "Processing"),
                ShippedOrders = orders.Count(o => o.Status == "Shipped"),
                DeliveredOrders = orders.Count(o => o.Status == "Delivered"),
                CancelledOrders = orders.Count(o => o.Status == "Cancelled"),
                TotalRevenue = orders.Where(o => o.Status == "Delivered").Sum(o => o.TotalAmount)
            };

            return summary;
        }
    }
}
