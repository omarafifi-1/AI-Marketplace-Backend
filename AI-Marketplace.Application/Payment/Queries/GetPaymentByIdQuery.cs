using AI_Marketplace.Application.Payment.DTOs;
using MediatR;

namespace AI_Marketplace.Application.Payment.Queries
{
    public class GetPaymentByIdQuery : IRequest<PaymentResponseDto?>
    {
        public int PaymentId { get; set; }
        public int UserId { get; set; }

        public GetPaymentByIdQuery(int paymentId, int userId)
        {
            PaymentId = paymentId;
            UserId = userId;
        }
    }
}
