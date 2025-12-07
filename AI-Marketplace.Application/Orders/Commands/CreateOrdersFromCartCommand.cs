using AI_Marketplace.Application.Orders.DTOs;
using MediatR;

namespace AI_Marketplace.Application.Orders.Commands
{
    public class CreateOrdersFromCartCommand : IRequest<List<OrderDto>>
    {
        public int UserId { get; }
        public int? CartId { get; }
        public string? ShippingAddress { get; }

        public CreateOrdersFromCartCommand(int userId, int? cartId, string? shippingAddress)
        {
            UserId = userId;
            CartId = cartId;
            ShippingAddress = shippingAddress;
        }
    }
}
