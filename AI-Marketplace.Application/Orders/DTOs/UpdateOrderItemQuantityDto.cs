using System.ComponentModel.DataAnnotations;

namespace AI_Marketplace.Application.Orders.DTOs
{
    public class UpdateOrderItemQuantityDto
    {
        [Required(ErrorMessage = "New quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int NewQuantity { get; set; }
    }
}
