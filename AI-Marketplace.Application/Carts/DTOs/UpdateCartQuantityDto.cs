using System.ComponentModel.DataAnnotations;

namespace AI_Marketplace.Application.Carts.DTOs
{
    public class UpdateCartQuantityDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Product ID must be greater than 0")]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }
}