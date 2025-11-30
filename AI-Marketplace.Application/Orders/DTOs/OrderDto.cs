using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Orders.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ShippingAddress { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveredAt { get; set; }

        public string BuyerName { get; set; } = string.Empty;
        public int StoreId { get; set; }
        public string StoreName { get; set; } = string.Empty;

        public List<OrderItemDto> Items { get; set; } = new();
    }
}
