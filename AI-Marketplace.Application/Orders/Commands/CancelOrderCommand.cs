using MediatR;

namespace AI_Marketplace.Application.Orders.Commands
{
    public class CancelOrderCommand : IRequest<bool>
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }

        public CancelOrderCommand(int orderId, int userId)
        {
            OrderId = orderId;
            UserId = userId;
        }
    }
}
