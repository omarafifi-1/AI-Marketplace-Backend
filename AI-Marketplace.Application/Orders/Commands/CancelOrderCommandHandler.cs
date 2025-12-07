using AI_Marketplace.Application.Common.Exceptions;
using AI_Marketplace.Application.Common.Interfaces;
using MediatR;

namespace AI_Marketplace.Application.Orders.Commands
{
    public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, bool>
    {
        private readonly IOrderRepository _orderRepository;

        public CancelOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<bool> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetOrderByIdAsync(request.OrderId, cancellationToken);
            if (order == null)
            {
                throw new NotFoundException(new Dictionary<string, string[]>
                {
                    { "Order", new[] { "Order not found or you don't have permission to cancel it." } }
                });
            }

            if (order.Status == "Delivered" || order.Status == "Cancelled")
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Order", new[] { $"Cannot cancel an order that is already {order.Status.ToLower()}." } }
                });
            }

            order.Status = "Cancelled";
            await _orderRepository.UpdateOrderAsync(order, cancellationToken);

            return true;
        }
    }
}
