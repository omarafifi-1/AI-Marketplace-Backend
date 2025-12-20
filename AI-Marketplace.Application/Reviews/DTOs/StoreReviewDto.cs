using System.ComponentModel.DataAnnotations;

namespace AI_Marketplace.Application.Reviews.DTOs
{
    public class StoreReviewDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "ProductId must be a valid id.")]
        public int StoreId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "BuyerId must be a valid id.")]
        public int BuyerId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }
        [MaxLength(500, ErrorMessage = "Comment cannot exceed 500 characters.")]
        public string Comment { get; set; }
    }
}