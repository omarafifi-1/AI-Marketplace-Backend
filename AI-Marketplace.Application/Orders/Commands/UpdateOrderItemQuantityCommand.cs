using AI_Marketplace.Application.Orders.DTOs;
using MediatR;

namespace AI_Marketplace.Application.Orders.Commands
{
    public class UpdateOrderItemQuantityCommand : IRequest<OrderDto>
    {
        public int OrderId { get; set; }
        public int OrderItemId { get; set; }
        public int UserId { get; set; }
        public int NewQuantity { get; set; }
    }
}
