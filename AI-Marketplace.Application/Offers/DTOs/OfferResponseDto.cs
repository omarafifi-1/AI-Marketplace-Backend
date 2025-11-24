using System;

namespace AI_Marketplace.Application.Offers.DTOs
{
    public class OfferResponseDto
    {
        public int Id { get; set; }

       public int CustomRequestId { get; set; }

        public int StoreId { get; set; }

        public string StoreName { get; set; } = string.Empty;

        public decimal ProposedPrice { get; set; }

        public int EstimatedDays { get; set; }

        public string? Message { get; set; }

        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; }
    }
}
