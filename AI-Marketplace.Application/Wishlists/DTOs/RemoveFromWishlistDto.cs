using System.ComponentModel.DataAnnotations;

namespace AI_Marketplace.Application.Wishlists.DTOs
{
    public class RemoveFromWishlistDto
    {
        [Required]
        public int ProductId { get; set; }
    }
}
