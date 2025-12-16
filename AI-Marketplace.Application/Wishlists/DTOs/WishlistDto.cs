using System.Collections.Generic;

namespace AI_Marketplace.Application.Wishlists.DTOs
{
    public class WishlistDto
    {
        public int UserId { get; set; }
        public int TotalItems { get; set; }
        public List<WishlistItemDto> Items { get; set; } = new List<WishlistItemDto>();
    }
}
