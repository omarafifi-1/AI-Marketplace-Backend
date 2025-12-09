using AI_Marketplace.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Stripe;
using Microsoft.Extensions.Options;
using AI_Marketplace.Infrastructure.ExternalServices.payment;

namespace AI_Marketplace.Infrastructure.ExternalServices
{
    public class StripePaymentService : IStripePaymentService
    {
        private readonly RefundService _refundService;

        public StripePaymentService(IOptions<StripeOptions> options, RefundService refundService)
        {
            StripeConfiguration.ApiKey = options.Value.SecretKey;
            _refundService = refundService;
        }

        public async Task<PaymentIntentResult> CreatePaymentIntent(long amount, string currency)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = amount,
                Currency = currency,
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                }
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);
            
            return new PaymentIntentResult
            {
                ClientSecret = paymentIntent.ClientSecret,
                PaymentIntentId = paymentIntent.Id
            };
        }

        public async Task<StripePaymentIntentInfo> GetPaymentIntent(string paymentIntentId)
        {
            var service = new PaymentIntentService();
            var paymentIntent = await service.GetAsync(paymentIntentId);
            
            return new StripePaymentIntentInfo
            {
                Id = paymentIntent.Id,
                Status = paymentIntent.Status,
                Amount = paymentIntent.Amount,
                Currency = paymentIntent.Currency,
                LatestChargeId = paymentIntent.LatestChargeId
            };
        }

        public async Task<string> CreateRefund(string paymentIntentId, long? refundAmount, CancellationToken cancellationToken = default)
        {
            var refundOptions = new RefundCreateOptions
            {
                PaymentIntent = paymentIntentId
            };

            var refund = await _refundService.CreateAsync(refundOptions, cancellationToken: cancellationToken);
            return refund.Id; 
        }
    }
}
