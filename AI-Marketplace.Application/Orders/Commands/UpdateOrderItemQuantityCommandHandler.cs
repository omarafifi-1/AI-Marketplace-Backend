using AI_Marketplace.Application.Common.Exceptions;
using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Orders.DTOs;
using AutoMapper;
using MediatR;

namespace AI_Marketplace.Application.Orders.Commands
{
    public class UpdateOrderItemQuantityCommandHandler : IRequestHandler<UpdateOrderItemQuantityCommand, OrderDto>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public UpdateOrderItemQuantityCommandHandler(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<OrderDto> Handle(UpdateOrderItemQuantityCommand request, CancellationToken cancellationToken)
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
                    { "Order", new[] { "Cannot modify items in an order that is not pending." } }
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

            var product = await _productRepository.GetByIdAsync(orderItem.ProductId, cancellationToken);
            if (product == null)
            {
                throw new NotFoundException(new Dictionary<string, string[]>
                {
                    { "Product", new[] { "Product not found." } }
                });
            }

            if (product.Stock < request.NewQuantity)
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Quantity", new[] { "Insufficient stock available." } }
                });
            }

            var oldTotalPrice = orderItem.TotalPrice;
            orderItem.Quantity = request.NewQuantity;
            orderItem.TotalPrice = request.NewQuantity * orderItem.UnitPrice;

            await _orderRepository.UpdateOrderItem(orderItem, cancellationToken);

            order.TotalAmount = order.TotalAmount - oldTotalPrice + orderItem.TotalPrice;
            await _orderRepository.UpdateOrderAsync(order, cancellationToken);

            var updatedOrder = await _orderRepository.GetOrderByIdAsync(request.OrderId, cancellationToken);
            return _mapper.Map<OrderDto>(updatedOrder);
        }
    }
}
