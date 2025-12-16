using AI_Marketplace.Application.Orders.DTOs;
using MediatR;

namespace AI_Marketplace.Application.Orders.Queries
{
    public class GetOrderByIdQuery : IRequest<OrderDto?>
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }

        public GetOrderByIdQuery(int orderId, int userId)
        {
            OrderId = orderId;
            UserId = userId;
        }
    }
}
