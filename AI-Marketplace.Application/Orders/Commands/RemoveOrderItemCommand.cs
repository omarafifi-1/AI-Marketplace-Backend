using MediatR;

namespace AI_Marketplace.Application.Orders.Commands
{
    public class RemoveOrderItemCommand : IRequest<bool>
    {
        public int OrderId { get; set; }
        public int OrderItemId { get; set; }
        public int UserId { get; set; }

        public RemoveOrderItemCommand(int orderId, int orderItemId, int userId)
        {
            OrderId = orderId;
            OrderItemId = orderItemId;
            UserId = userId;
        }
    }
}
