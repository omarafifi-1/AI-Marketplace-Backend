using System.Collections.Generic;

namespace AI_Marketplace.Application.Orders.DTOs
{
    public class CreateOrdersResponse
    {
        public int MasterOrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderDto> Orders { get; set; } = new();
    }
}
