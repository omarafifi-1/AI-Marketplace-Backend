using System.ComponentModel.DataAnnotations;

namespace AI_Marketplace.Application.Wishlists.DTOs
{
    public class AddToWishlistDto
    {
        [Required]
        public int ProductId { get; set; }
    }
}
