using AI_Marketplace.Application.Payment.DTOs;
using MediatR;

namespace AI_Marketplace.Application.Payment.Commands
{
    public class RefundPaymentCommand : IRequest<PaymentResponseDto>
    {
        public int PaymentId { get; set; }
        public long RefundAmount { get; set; }
        public string? RefundReason { get; set; }
        public int UserId { get; set; } 

        public RefundPaymentCommand(int paymentId, long refundAmount, string? refundReason, int userId)
        {
            PaymentId = paymentId;
            RefundAmount = refundAmount;
            RefundReason = refundReason;
            UserId = userId;
        }
    }
}
