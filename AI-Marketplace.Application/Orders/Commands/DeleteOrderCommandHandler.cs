using AI_Marketplace.Application.Common.Exceptions;
using AI_Marketplace.Application.Common.Interfaces;
using MediatR;

namespace AI_Marketplace.Application.Orders.Commands
{
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, bool>
    {
        private readonly IOrderRepository _orderRepository;

        public DeleteOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<bool> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetOrderByIdAsync(request.OrderId, cancellationToken);
            if (order == null)
            {
                throw new NotFoundException(new Dictionary<string, string[]>
                {
                    { "Order", new[] { "Order not found." } }
                });
            }

            var deleted = await _orderRepository.CancelOrderByOrderIdAsync(request.OrderId, cancellationToken);
            return deleted;
        }
    }
}
