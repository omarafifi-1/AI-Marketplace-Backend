using AI_Marketplace.Application.Payment.DTOs;
using MediatR;
using System.Collections.Generic;

namespace AI_Marketplace.Application.Payment.Queries
{
    public class GetPaymentsByOrderIdQuery : IRequest<List<PaymentResponseDto>>
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }

        public GetPaymentsByOrderIdQuery(int orderId, int userId)
        {
            OrderId = orderId;
            UserId = userId;
        }
    }
}
