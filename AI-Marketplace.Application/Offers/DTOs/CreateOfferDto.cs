using System.ComponentModel.DataAnnotations;

namespace AI_Marketplace.Application.Offers.DTOs
{
    public class CreateOfferDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "CustomRequestId must be a positive integer.")]
        public int CustomRequestId { get; set; }

        [Required]
        [Range(0.01, 999999.99, ErrorMessage = "ProposedPrice must be between 0.01 and 999,999.99.")]
        public decimal ProposedPrice { get; set; }

        [Required]
        [Range(1, 365, ErrorMessage = "EstimatedDays must be between 1 and 365.")]
        public int EstimatedDays { get; set; }

        [MaxLength(500, ErrorMessage = "Message cannot exceed 500 characters.")]
        public string? Message { get; set; }
    }
}
