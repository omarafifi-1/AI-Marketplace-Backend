using System.ComponentModel.DataAnnotations;

namespace AI_Marketplace.Application.Orders.DTOs
{
    public class ChangeOrderStatusDto
    {
        [Required(ErrorMessage = "Status is required")]
        [RegularExpression("^(Pending|Processing|Shipped|Delivered|Cancelled)$", 
            ErrorMessage = "Status must be one of: Pending, Processing, Shipped, Delivered, Cancelled")]
        public string Status { get; set; } = string.Empty;
    }
}
