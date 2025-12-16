using AI_Marketplace.Application.Common.Exceptions;
using AI_Marketplace.Application.Common.Interfaces;
using MediatR;

namespace AI_Marketplace.Application.Orders.Commands
{
    public class RemoveOrderItemCommandHandler : IRequestHandler<RemoveOrderItemCommand, bool>
    {
        private readonly IOrderRepository _orderRepository;

        public RemoveOrderItemCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<bool> Handle(RemoveOrderItemCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetOrderByIdAsync(request.OrderId, cancellationToken);
            if (order == null || order.BuyerId != request.UserId)
            {
                throw new NotFoundException(new Dictionary<string, string[]>
                {
                    { "Order", new[] { "Order not found or you don't have permission to modify it." } }
                });
            }

            if (order.Status != "Pending")
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Order", new[] { "Cannot remove items from an order that is not pending." } }
                });
            }

            var orderItem = await _orderRepository.GetOrderItemById(request.OrderItemId, cancellationToken);
            if (orderItem == null || orderItem.OrderId != request.OrderId)
            {
                throw new NotFoundException(new Dictionary<string, string[]>
                {
                    { "OrderItem", new[] { "Order item not found in this order." } }
                });
            }

            var removed = await _orderRepository.RemoveOrderItem(request.OrderItemId, cancellationToken);

            if (removed)
            {
                order.TotalAmount -= orderItem.TotalPrice;
                await _orderRepository.UpdateOrderAsync(order, cancellationToken);
            }

            return removed;
        }
    }
}
