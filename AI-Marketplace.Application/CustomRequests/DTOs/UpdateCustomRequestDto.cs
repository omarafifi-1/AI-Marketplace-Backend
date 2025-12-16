using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AI_Marketplace.Application.CustomRequests.DTOs
{
    public class UpdateCustomRequestDto
    {
        [StringLength(2000, MinimumLength = 10)]
        public string? Description { get; set; }
        public int? CategoryId { get; set; }

       
        [StringLength(700, ErrorMessage = "ImageUrl cannot exceed 700 characters")]
        public string? ImageUrl { get; set; }

        [Range(1, 999999, ErrorMessage = "Budget must be between 1 and 999,999")]
        public decimal? Budget { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Deadline { get; set; }
    }
}
