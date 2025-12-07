using AI_Marketplace.Application.Common.Exceptions;
using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Orders.DTOs;
using AI_Marketplace.Domain.Entities;
using AutoMapper;
using MediatR;

namespace AI_Marketplace.Application.Orders.Commands
{
    public class AddOrderItemCommandHandler : IRequestHandler<AddOrderItemCommand, OrderDto>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public AddOrderItemCommandHandler(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<OrderDto> Handle(AddOrderItemCommand request, CancellationToken cancellationToken)
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
                    { "Order", new[] { "Cannot add items to an order that is not pending." } }
                });
            }

            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
            if (product == null)
            {
                throw new NotFoundException(new Dictionary<string, string[]>
                {
                    { "Product", new[] { "Product not found." } }
                });
            }

            if (product.Stock < request.Quantity)
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Quantity", new[] { "Insufficient stock available." } }
                });
            }

            var orderItem = new OrderItem
            {
                OrderId = request.OrderId,
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                UnitPrice = product.Price,
                TotalPrice = request.Quantity * product.Price
            };

            await _orderRepository.AddOrderItem(orderItem, cancellationToken);

            order.TotalAmount += orderItem.TotalPrice;
            await _orderRepository.UpdateOrderAsync(order, cancellationToken);

            var updatedOrder = await _orderRepository.GetOrderByIdAsync(request.OrderId, cancellationToken);
            return _mapper.Map<OrderDto>(updatedOrder);
        }
    }
}
