using Microsoft.AspNetCore.Mvc;
using Stripe;
using Microsoft.Extensions.Options;
using AI_Marketplace.Infrastructure.ExternalServices.payment;
using AI_Marketplace.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace AI_Marketplace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly string _webhookSecret;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<WebhookController> _logger;

        public WebhookController(
            IOptions<StripeOptions> stripeOptions,
            IPaymentRepository paymentRepository,
            IOrderRepository orderRepository,
            ILogger<WebhookController> logger)
        {
            _webhookSecret = stripeOptions.Value.WebhookSecret;
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
            _logger = logger;
        }

        [HttpPost("stripe")]
        public async Task<IActionResult> HandleStripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripeSignature = Request.Headers["Stripe-Signature"];

                if (string.IsNullOrEmpty(stripeSignature))
                {
                    _logger.LogWarning("Webhook received without Stripe signature");
                    return BadRequest("Missing Stripe signature");
                }

                _logger.LogInformation("Processing Stripe webhook event");

                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    stripeSignature,
                    _webhookSecret,
                    throwOnApiVersionMismatch: false
                );

                _logger.LogInformation("Webhook event type: {EventType}, ID: {EventId}",
                    stripeEvent.Type, stripeEvent.Id);

                switch (stripeEvent.Type)
                {
                    case "payment_intent.succeeded":
                        await HandlePaymentIntentSucceeded(stripeEvent);
                        break;

                    case "payment_intent.payment_failed":
                        await HandlePaymentIntentFailed(stripeEvent);
                        break;

                    case "charge.refunded":
                        await HandleChargeRefunded(stripeEvent);
                        break;

                    default:
                        _logger.LogInformation("Unhandled event type: {EventType}", stripeEvent.Type);
                        break;
                }

                return Ok();
            }
            catch (StripeException ex)
            {
                _logger.LogWarning(ex, "Webhook signature verification failed");
                return BadRequest($"Webhook signature verification failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing webhook");
                return StatusCode(500, "Error processing webhook");
            }
        }

        private async Task HandlePaymentIntentSucceeded(Event stripeEvent)
        {
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            if (paymentIntent == null)
            {
                _logger.LogWarning("PaymentIntent object is null in webhook event");
                return;
            }

            _logger.LogInformation(
                "Processing payment_intent.succeeded: PaymentIntentId={PaymentIntentId}, Amount={Amount}",
                paymentIntent.Id,
                paymentIntent.Amount);

            var payment = await _paymentRepository.GetByPaymentIntentIdAsync(paymentIntent.Id);
            if (payment == null)
            {
                _logger.LogWarning(
                    "Payment not found for PaymentIntentId: {PaymentIntentId}",
                    paymentIntent.Id);
                return;
            }

            if (payment.Status == Domain.enums.PaymentStatus.Succeeded)
            {
                _logger.LogInformation(
                    "Payment already succeeded (idempotent): PaymentId={PaymentId}",
                    payment.Id);
                return;
            }

            payment.Status = Domain.enums.PaymentStatus.Succeeded;
            payment.TransactionId = paymentIntent.LatestChargeId;
            payment.ProcessedAt = DateTime.UtcNow;
            payment.CompletedAt = DateTime.UtcNow;
            payment.PaymentGatewayResponse = paymentIntent.ToJson();

            await _paymentRepository.UpdateAsync(payment);

            _logger.LogInformation(
                "Payment updated to Succeeded: PaymentId={PaymentId}, OrderId={OrderId}",
                payment.Id,
                payment.OrderId);
        }

        private async Task HandlePaymentIntentFailed(Event stripeEvent)
        {
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            if (paymentIntent == null)
            {
                _logger.LogWarning("PaymentIntent object is null in webhook event");
                return;
            }

            _logger.LogInformation(
                "Processing payment_intent.payment_failed: PaymentIntentId={PaymentIntentId}",
                paymentIntent.Id);

            var payment = await _paymentRepository.GetByPaymentIntentIdAsync(paymentIntent.Id);
            if (payment == null)
            {
                _logger.LogWarning(
                    "Payment not found for PaymentIntentId: {PaymentIntentId}",
                    paymentIntent.Id);
                return;
            }

            if (payment.Status == Domain.enums.PaymentStatus.Failed)
            {
                _logger.LogInformation(
                    "Payment already failed (idempotent): PaymentId={PaymentId}",
                    payment.Id);
                return;
            }

            payment.Status = Domain.enums.PaymentStatus.Failed;
            payment.FailureReason = paymentIntent.LastPaymentError?.Message ?? "Payment failed";
            payment.ProcessedAt = DateTime.UtcNow;
            payment.PaymentGatewayResponse = paymentIntent.ToJson();

            await _paymentRepository.UpdateAsync(payment);

            _logger.LogInformation(
                "Payment updated to Failed: PaymentId={PaymentId}, Reason={Reason}",
                payment.Id,
                payment.FailureReason);
        }

        private async Task HandleChargeRefunded(Event stripeEvent)
        {
            var charge = stripeEvent.Data.Object as Charge;
            if (charge == null)
            {
                _logger.LogWarning("Charge object is null in webhook event");
                return;
            }

            _logger.LogInformation(
                "Processing charge.refunded: ChargeId={ChargeId}, RefundAmount={RefundAmount}",
                charge.Id,
                charge.AmountRefunded);

            var payment = await _paymentRepository.GetByTransactionIdAsync(charge.Id);
            if (payment == null)
            {
                _logger.LogWarning(
                    "Payment not found for TransactionId: {TransactionId}",
                    charge.Id);
                return;
            }

            payment.RefundedAmount = charge.AmountRefunded;
            payment.RefundedAt = DateTime.UtcNow;
            payment.PaymentGatewayResponse = charge.ToJson();

            if (charge.AmountRefunded >= payment.Amount)
            {
                payment.Status = Domain.enums.PaymentStatus.Refunded;
                _logger.LogInformation("Full refund processed for PaymentId={PaymentId}", payment.Id);
            }
            else
            {
                payment.Status = Domain.enums.PaymentStatus.PartiallyRefunded;
                _logger.LogInformation(
                    "Partial refund processed for PaymentId={PaymentId}: {RefundedAmount}/{TotalAmount}",
                    payment.Id,
                    charge.AmountRefunded,
                    payment.Amount);
            }

            await _paymentRepository.UpdateAsync(payment);

            _logger.LogInformation(
                "Payment refund status updated: PaymentId={PaymentId}, Status={Status}",
                payment.Id,
                payment.Status);
        }
    }
}
