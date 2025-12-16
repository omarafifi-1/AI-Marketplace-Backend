using System.ComponentModel.DataAnnotations;

namespace AI_Marketplace.Application.Carts.DTOs
{
    public class RemoveProductFromCartDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Product ID must be greater than 0")]
        public int ProductId { get; set; }
    }
}