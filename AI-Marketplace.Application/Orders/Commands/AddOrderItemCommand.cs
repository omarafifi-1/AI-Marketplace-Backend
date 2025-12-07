using AI_Marketplace.Application.Orders.DTOs;
using MediatR;

namespace AI_Marketplace.Application.Orders.Commands
{
    public class AddOrderItemCommand : IRequest<OrderDto>
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
