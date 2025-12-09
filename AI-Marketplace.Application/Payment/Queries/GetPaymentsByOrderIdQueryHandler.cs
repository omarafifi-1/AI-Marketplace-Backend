using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Payment.DTOs;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AI_Marketplace.Application.Payment.Queries
{
    public class GetPaymentsByOrderIdQueryHandler : IRequestHandler<GetPaymentsByOrderIdQuery, List<PaymentResponseDto>>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;

        public GetPaymentsByOrderIdQueryHandler(IPaymentRepository paymentRepository, IOrderRepository orderRepository)
        {
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
        }

        public async Task<List<PaymentResponseDto>> Handle(GetPaymentsByOrderIdQuery request, CancellationToken cancellationToken)
        {
            // Verify order exists and user has access
            var order = await _orderRepository.GetOrderByIdAsync(request.OrderId, cancellationToken);
            if (order == null)
            {
                return new List<PaymentResponseDto>();
            }

            // TODO: Add proper authorization check
            // if (order.BuyerId != request.UserId) throw new UnauthorizedAccessException();

            var payments = await _paymentRepository.GetByOrderIdAsync(request.OrderId, cancellationToken);

            return payments.Select(p => new PaymentResponseDto
            {
                Id = p.Id,
                OrderId = p.OrderId,
                PaymentMethod = p.PaymentMethod.ToString(),
                PaymentIntentId = p.PaymentIntentId,
                TransactionId = p.TransactionId,
                Amount = p.Amount,
                Currency = p.Currency,
                Status = p.Status.ToString(),
                FailureReason = p.FailureReason,
                RefundedAmount = p.RefundedAmount,
                RefundTransactionId = p.RefundTransactionId,
                RefundedAt = p.RefundedAt,
                RefundReason = p.RefundReason,
                CreatedAt = p.CreatedAt,
                ProcessedAt = p.ProcessedAt,
                CompletedAt = p.CompletedAt
            }).ToList();
        }
    }
}
