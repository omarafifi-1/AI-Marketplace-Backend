using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AI_Marketplace.Application.Payment.DTO
{
    public class PaymentDto
    {
        [Required(ErrorMessage = "Master Order ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Master Order ID must be greater than 0.")]
        public int MasterOrderId { get; set; }

        /*
        [Required(ErrorMessage = "Amount is required.")]
        [Range(1, long.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public long Amount { get; set; }
        */

        [Required(ErrorMessage = "Method Of Payment is required")]
        public string ChosenMethodOfPayment { get; set; }

        [Required(ErrorMessage = "Currency code is required.")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency code must be exactly 3 characters (e.g., USD, EUR).")]
        [RegularExpression("^[A-Z]{3}$", ErrorMessage = "Currency code must be 3 uppercase letters (e.g., USD, EUR, GBP).")]
        public string? CurrencyCode { get; set; } = "EGP";
    }
}
