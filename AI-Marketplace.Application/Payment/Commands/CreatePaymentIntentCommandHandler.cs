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
        private readonly IMasterOrderRepository _masterOrderRepository;

        public CreatePaymentIntentCommandHandler(
            IStripePaymentService stripe,
            IPaymentRepository paymentRepository,
            IMasterOrderRepository masterOrderRepository)
        {
            _stripe = stripe;
            _paymentRepository = paymentRepository;
            _masterOrderRepository = masterOrderRepository;
        }

        public async Task<string> Handle(CreatePaymentIntentCommand request, CancellationToken cancellationToken)
        {
            // Verify master order exists and get buyer info
            var masterOrder = await _masterOrderRepository.GetByIdAsync(request.MasterOrderId, cancellationToken);
            if (masterOrder == null)
            {
                throw new InvalidOperationException($"Master Order with ID {request.MasterOrderId} not found.");
            }

            // Validate inputs (amount is in major units like USD dollars)
            if (request.Amount <= 0)
            {
                throw new InvalidOperationException("Amount must be greater than 0.");
            }
            if (string.IsNullOrWhiteSpace(request.Currency) || request.Currency.Length != 3)
            {
                throw new InvalidOperationException("Currency must be a 3-letter ISO code (e.g., USD, EUR).");
            }

            var currency = request.Currency.ToUpperInvariant();
            var amountInSmallestUnit = ConvertToSmallestUnit(request.Amount, currency);

            try
            {
                // Create intent
                var intent = await _stripe.CreatePaymentIntent(amountInSmallestUnit, currency);

                var payment = new Domain.Entities.Payment
                {
                    MasterOrderId = request.MasterOrderId,
                    PaymentMethod = PaymentMethod.Stripe,
                    PaymentIntentId = intent.PaymentIntentId,
                    Amount = request.Amount, 
                    Currency = currency,
                    Status = PaymentStatus.Pending,
                    CustomerEmail = masterOrder.Buyer?.Email,
                    CustomerName = $"{masterOrder.Buyer?.FirstName} {masterOrder.Buyer?.LastName}".Trim(),
                    CreatedAt = DateTime.UtcNow
                };

                await _paymentRepository.CreateAsync(payment, cancellationToken);

                return intent.ClientSecret;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Payment provider error: {ex.Message}");
            }
        }


        //Utility method 
        private static long ConvertToSmallestUnit(long amountMajorUnits, string currency)
        {
            return currency switch
            {
                "JPY" => amountMajorUnits,
                "KRW" => amountMajorUnits,
                _ => checked(amountMajorUnits * 100)
            };
        }
    }
}
