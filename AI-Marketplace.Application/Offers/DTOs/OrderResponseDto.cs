using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Offers.DTOs
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        
        public int BuyerId { get; set; }
        
        public string BuyerName { get; set; } = string.Empty;
        
        public int StoreId { get; set; }
        
        public string StoreName { get; set; } = string.Empty;
        
        public int? OfferId { get; set; }
        
        public decimal TotalAmount { get; set; }
        
        public string Status { get; set; } = "Pending";
        
        public string ShippingAddress { get; set; } = string.Empty;
        
        public DateTime OrderDate { get; set; }
        
        public DateTime? DeliveredAt { get; set; }
        
        public OfferResponseDto? Offer { get; set; }
    }
}
