using System;

namespace AI_Marketplace.Application.Wishlists.DTOs
{
    public class WishlistItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ProductDescription { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public int Stock { get; set; }
        public decimal Rating { get; set; }
        public string? PrimaryImageUrl { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; } = string.Empty;
        public DateTime AddedOn { get; set; }
    }
}
