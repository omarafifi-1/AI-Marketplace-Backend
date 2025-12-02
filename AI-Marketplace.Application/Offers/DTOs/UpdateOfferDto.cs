using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AI_Marketplace.Application.Offers.DTOs
{
    public class UpdateOfferDto
    {
        [Required]
        [Range(0.01, 999999.99, ErrorMessage = "ProposedPrice must be between 0.01 and 999,999.99.")]
        public decimal ProposedPrice { get; set; }

        [Required]
        [Range(1, 365, ErrorMessage = "EstimatedDays must be between 1 and 365.")]
        public int EstimatedDays { get; set; }

        public string? Message { get; set; }
    }
}
