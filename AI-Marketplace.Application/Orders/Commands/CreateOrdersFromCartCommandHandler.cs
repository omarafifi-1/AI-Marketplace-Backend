using AI_Marketplace.Application.Common.Exceptions;
using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Orders.DTOs;
using AI_Marketplace.Domain.Entities;
using AutoMapper;
using MediatR;

namespace AI_Marketplace.Application.Orders.Commands
{
    public class CreateOrdersFromCartCommandHandler : IRequestHandler<CreateOrdersFromCartCommand, List<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;

        public CreateOrdersFromCartCommandHandler(
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _mapper = mapper;
        }

        public async Task<List<OrderDto>> Handle(CreateOrdersFromCartCommand request, CancellationToken cancellationToken)
        {
            var cart = request.CartId.HasValue
                ? await _cartRepository.GetCartByCartIdAsync(request.CartId.Value, cancellationToken)
                : await _cartRepository.GetCartByUserIdAsync(request.UserId, cancellationToken);

            if (cart == null || cart.CartItems.Count == 0)
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Cart", new[] { "Cart is empty or not found." } }
                });
            }

            var itemsByStore = cart.CartItems
                .GroupBy(ci => ci.Product.StoreId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var createdOrderDtos = new List<OrderDto>();

            foreach (var KeyValuePair in itemsByStore)
            {
                var storeId = KeyValuePair.Key;

                var order = new Order
                {
                    BuyerId = request.UserId,
                    StoreId = storeId,
                    ShippingAddress = request.ShippingAddress,
                    Status = "Pending",
                    OrderDate = DateTime.UtcNow,
                    OrderItems = new List<OrderItem>()
                };

                decimal totalAmount = 0;
                foreach (var cartItem in KeyValuePair.Value)
                {
                    var unitPrice = cartItem.UnitPrice; 
                    var quantity = cartItem.Quantity;

                    var orderItem = new OrderItem
                    {
                        ProductId = cartItem.ProductId,
                        Quantity = quantity,
                        UnitPrice = unitPrice,
                        TotalPrice = quantity * unitPrice
                    };

                    order.OrderItems.Add(orderItem);
                    totalAmount += orderItem.TotalPrice;
                }

                order.TotalAmount = totalAmount;

                var createdOrder = await _orderRepository.CreateAsync(order, cancellationToken);
                var result = await _orderRepository.GetOrderByIdAsync(createdOrder.Id, cancellationToken);
                createdOrderDtos.Add(_mapper.Map<OrderDto>(result));
            }

            await _cartRepository.ClearCartItemsAsync(cart.Id, cancellationToken);

            return createdOrderDtos;
        }
    }
}
