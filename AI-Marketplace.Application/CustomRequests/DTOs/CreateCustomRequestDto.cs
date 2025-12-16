using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AI_Marketplace.Application.CustomRequests.DTOs
{
    public class CreateCustomRequestDto 
    {
        [Required(ErrorMessage = "Description is required")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 2000 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "CategoryId is required")]
        public int CategoryId { get; set; }

       
        [StringLength(700, ErrorMessage = "ImageUrl cannot exceed 700 characters")]
        public string? ImageUrl { get; set; }

        [Range(1, 999999, ErrorMessage = "Budget must be between 0.01 and 999,999.99")]
        public decimal? Budget { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Deadline { get; set; }
    }
}
