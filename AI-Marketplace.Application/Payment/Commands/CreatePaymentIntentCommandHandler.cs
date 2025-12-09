using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Domain.Entities;
using AI_Marketplace.Domain.enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Payment.Commands
{
    public class CreatePaymentIntentCommandHandler : IRequestHandler<CreatePaymentIntentCommand, string>
    {
        private readonly IStripePaymentService _stripe;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;

        public CreatePaymentIntentCommandHandler(
            IStripePaymentService stripe,
            IPaymentRepository paymentRepository,
            IOrderRepository orderRepository)
        {
            _stripe = stripe;
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
        }

        public async Task<string> Handle(CreatePaymentIntentCommand request, CancellationToken cancellationToken)
        {
            // Verify order exists
            var order = await _orderRepository.GetOrderByIdAsync(request.OrderId, cancellationToken);
            if (order == null)
            {
                throw new InvalidOperationException($"Order with ID {request.OrderId} not found.");
            }


            var paymentIntentResult = await _stripe.CreatePaymentIntent(request.Amount, request.Currency);


            var payment = new Domain.Entities.Payment
            {
                OrderId = request.OrderId,
                PaymentMethod = PaymentMethod.Stripe,
                PaymentIntentId = paymentIntentResult.PaymentIntentId,
                Amount = request.Amount,
                Currency = request.Currency,
                Status = PaymentStatus.Pending,
                CustomerEmail = order.Buyer?.Email,
                CustomerName = $"{order.Buyer?.FirstName} {order.Buyer?.LastName}".Trim(),
                CreatedAt = DateTime.UtcNow
            };

            await _paymentRepository.CreateAsync(payment, cancellationToken);

            return paymentIntentResult.ClientSecret;
        }
    }
}
