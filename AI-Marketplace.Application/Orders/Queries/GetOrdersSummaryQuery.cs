using MediatR;

namespace AI_Marketplace.Application.Orders.Queries
{
    public class GetOrdersSummaryQuery : IRequest<OrdersSummaryDto>
    {
        public int UserId { get; set; }
        public string? UserRole { get; set; }

        public GetOrdersSummaryQuery(int userId, string? userRole)
        {
            UserId = userId;
            UserRole = userRole;
        }
    }

    public class OrdersSummaryDto
    {
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int ProcessingOrders { get; set; }
        public int ShippedOrders { get; set; }
        public int DeliveredOrders { get; set; }
        public int CancelledOrders { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
