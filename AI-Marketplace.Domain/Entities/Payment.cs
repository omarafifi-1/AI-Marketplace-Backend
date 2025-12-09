using AI_Marketplace.Domain.enums;
using System;

namespace AI_Marketplace.Domain.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        
        // Payment Gateway Information
        public PaymentMethod PaymentMethod { get; set; }
        public string? PaymentIntentId { get; set; } // Stripe Payment Intent ID (pi_xxxxx)
        public string? TransactionId { get; set; } // Stripe Charge ID (ch_xxxxx) - set after successful charge
        public string? PaymentGatewayResponse { get; set; } // JSON response from payment gateway
        
        // Payment Details
        public long Amount { get; set; } // Amount in smallest currency unit (e.g., cents)
        public string Currency { get; set; } = "usd";
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public string? FailureReason { get; set; }
        
        // Refund Information
        public long? RefundedAmount { get; set; } // Amount refunded in smallest currency unit
        public string? RefundTransactionId { get; set; }
        public DateTime? RefundedAt { get; set; }
        public string? RefundReason { get; set; }
        
        // Metadata
        public string? CustomerEmail { get; set; }
        public string? CustomerName { get; set; }
        public string? BillingAddress { get; set; }
        
        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        
        // Navigation Properties
        public Order Order { get; set; } = null!;
    }
}
