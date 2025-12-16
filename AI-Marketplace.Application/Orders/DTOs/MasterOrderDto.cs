using System;
using System.Collections.Generic;

namespace AI_Marketplace.Application.Orders.DTOs
{
    public class MasterOrderDto
    {
        public int Id { get; set; }
        public int BuyerId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ShippingAddress { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public List<OrderDto> ChildOrders { get; set; } = new();
    }
}
