using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Payment.DTOs;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AI_Marketplace.Application.Payment.Commands
{
    public class RefundPaymentCommandHandler : IRequestHandler<RefundPaymentCommand, PaymentResponseDto>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IStripePaymentService _stripeService;

        public RefundPaymentCommandHandler(IPaymentRepository paymentRepository, IStripePaymentService stripeService)
        {
            _paymentRepository = paymentRepository;
            _stripeService = stripeService;
        }

        public async Task<PaymentResponseDto> Handle(RefundPaymentCommand request, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
            
            if (payment == null)
            {
                throw new InvalidOperationException($"Payment with ID {request.PaymentId} not found.");
            }

            if (payment.Status != Domain.enums.PaymentStatus.Succeeded)
            {
                throw new InvalidOperationException("Only succeeded payments can be refunded.");
            }

            // Call Stripe refund API when implemented
            var refundTransactionId = await _stripeService.CreateRefund(payment.PaymentIntentId, request.RefundAmount);

            // Process refund in database
            var success = await _paymentRepository.ProcessRefundAsync(
                request.PaymentId,
                request.RefundAmount,
                refundTransactionId, 
                request.RefundReason,
                cancellationToken
            );

            if (!success)
            {
                throw new InvalidOperationException("Failed to process refund.");
            }

            // Reload payment
            payment = await _paymentRepository.GetByIdAsync(payment.Id, cancellationToken);

            return new PaymentResponseDto
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                PaymentMethod = payment.PaymentMethod.ToString(),
                PaymentIntentId = payment.PaymentIntentId,
                TransactionId = payment.TransactionId,
                Amount = payment.Amount,
                Currency = payment.Currency,
                Status = payment.Status.ToString(),
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
