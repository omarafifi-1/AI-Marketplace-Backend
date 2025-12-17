using System.Threading;
using System.Threading.Tasks;

namespace AI_Marketplace.Application.Common.Interfaces
{
    public interface IStripePaymentService
    {
        Task<PaymentIntentResult> CreatePaymentIntent(long amount, string currency);
        Task<StripePaymentIntentInfo> GetPaymentIntent(string paymentIntentId);
        Task<string> CreateRefund(string paymentIntentId, long? refundAmount, CancellationToken cancellationToken = default);
    }

    public class PaymentIntentResult
    {
        public string ClientSecret { get; set; } = string.Empty;
        public string PaymentIntentId { get; set; } = string.Empty;
    }

    public class StripePaymentIntentInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public long Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string? ClientSecret { get; set; }
        public string? LatestChargeId { get; set; }
    }
}
