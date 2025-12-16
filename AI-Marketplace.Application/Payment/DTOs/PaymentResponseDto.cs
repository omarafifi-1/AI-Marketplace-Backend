using System;

namespace AI_Marketplace.Application.Payment.DTOs
{
    public class PaymentResponseDto
    {
        public int Id { get; set; }
        public int? OrderId { get; set; }
        public int? MasterOrderId { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string? PaymentIntentId { get; set; }
        public string? TransactionId { get; set; }
        public long Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? FailureReason { get; set; }
        public long? RefundedAmount { get; set; }
        public string? RefundTransactionId { get; set; }
        public DateTime? RefundedAt { get; set; }
        public string? RefundReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
