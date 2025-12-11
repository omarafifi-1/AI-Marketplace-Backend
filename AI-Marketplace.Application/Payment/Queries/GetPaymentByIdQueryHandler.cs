using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Payment.DTOs;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AI_Marketplace.Application.Payment.Queries
{
    public class GetPaymentByIdQueryHandler : IRequestHandler<GetPaymentByIdQuery, PaymentResponseDto?>
    {
        private readonly IPaymentRepository _paymentRepository;

        public GetPaymentByIdQueryHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<PaymentResponseDto?> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
            
            if (payment == null)
            {
                return null;
            }

            // Authorization check - only allow access to own payments or admin
            // TODO: Add proper role-based authorization
            if (payment.Order?.BuyerId != request.UserId)
            {
                throw new UnauthorizedAccessException("You don't have access to this payment.");
            }

            return new PaymentResponseDto
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                MasterOrderId = payment.MasterOrderId,
                PaymentMethod = payment.PaymentMethod.ToString(),
                PaymentIntentId = payment.PaymentIntentId,
                TransactionId = payment.TransactionId,
                Amount = payment.Amount,
                Currency = payment.Currency,
                Status = payment.Status.ToString(),
                FailureReason = payment.FailureReason,
                RefundedAmount = payment.RefundedAmount,
                RefundTransactionId = payment.RefundTransactionId,
                RefundedAt = payment.RefundedAt,
                RefundReason = payment.RefundReason,
                CreatedAt = payment.CreatedAt,
                ProcessedAt = payment.ProcessedAt,
                CompletedAt = payment.CompletedAt
            };
        }
    }
}
