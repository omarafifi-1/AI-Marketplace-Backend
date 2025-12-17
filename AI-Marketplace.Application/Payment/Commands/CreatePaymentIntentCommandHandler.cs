using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Domain.Entities;
using AI_Marketplace.Domain.enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
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
            //Validation Region
            #region
            var masterOrder = await _masterOrderRepository.GetByIdAsync(request.MasterOrderId, cancellationToken);
            if (masterOrder == null || (masterOrder.Buyer.Id != request.UserId))
            {
                throw new InvalidOperationException($"Master Order with ID {request.MasterOrderId} not found or doesn't belong to this user");
            }


            if (masterOrder.Status != "Pending")
            {
                throw new InvalidOperationException($"Cannot create payment intent for MasterOrder with status '{masterOrder.Status}'. Only 'Pending' orders can be paid.");
            }

            var existingPayments = await _paymentRepository.GetByMasterOrderIdAsync(request.MasterOrderId, cancellationToken);
            var existingPendingPayment = existingPayments.FirstOrDefault(p => p.Status == PaymentStatus.Pending && !string.IsNullOrEmpty(p.PaymentIntentId));
            
            if (existingPendingPayment != null)
            {
                var existingIntent = await _stripe.GetPaymentIntent(existingPendingPayment.PaymentIntentId);
                if (existingIntent != null && !string.IsNullOrEmpty(existingIntent.ClientSecret))
                {
                    return existingIntent.ClientSecret;
                }
            }

            // Validate currency
            if (string.IsNullOrWhiteSpace(request.Currency) || request.Currency.Length != 3)
            {
                throw new InvalidOperationException("Currency must be a 3-letter ISO code (e.g., USD, EUR).");
            }
            #endregion


            var currency = request.Currency.ToUpperInvariant();
            
            // Convert decimal TotalAmount to long (smallest currency unit)
            var amountInSmallestUnit = ConvertToSmallestUnit((long)masterOrder.TotalAmount, currency);

            try
            {
                // Create intent
                if (request.MehtodOfPayment != PaymentMethod.COD.ToString())
                {
                    var intent = await _stripe.CreatePaymentIntent(amountInSmallestUnit, currency);

                    var payment = new Domain.Entities.Payment
                    {
                        MasterOrderId = request.MasterOrderId,
                        PaymentMethod = PaymentMethod.Stripe,
                        PaymentIntentId = intent.PaymentIntentId,
                        Amount = amountInSmallestUnit,
                        Currency = currency,
                        Status = PaymentStatus.Pending,
                        CustomerEmail = masterOrder.Buyer?.Email,
                        CustomerName = $"{masterOrder.Buyer?.FirstName} {masterOrder.Buyer?.LastName}".Trim(),
                        CreatedAt = DateTime.UtcNow
                    };

                    await _paymentRepository.CreateAsync(payment, cancellationToken);

                    return intent.ClientSecret;
                }
                var paymentCOD = new Domain.Entities.Payment
                {
                    MasterOrderId = request.MasterOrderId,
                    PaymentMethod = PaymentMethod.COD,
                    Amount = amountInSmallestUnit,
                    Currency = currency,
                    Status = PaymentStatus.Pending,
                    CustomerEmail = masterOrder.Buyer?.Email,
                    CustomerName = $"{masterOrder.Buyer?.FirstName} {masterOrder.Buyer?.LastName}".Trim(),
                    CreatedAt = DateTime.UtcNow
                };

                await _paymentRepository.CreateAsync(paymentCOD, cancellationToken);

                return "Awaiting Payment...";
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
