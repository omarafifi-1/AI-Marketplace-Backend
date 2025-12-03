using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AI_Marketplace.Application.Offers.DTOs
{
    public class AcceptOfferDto
    {
        [Required(ErrorMessage = "Shipping address is required.")]
        [MaxLength(500, ErrorMessage = "Shipping address cannot exceed 500 characters.")]
        public string ShippingAddress { get; set; } = string.Empty;
    }
}
