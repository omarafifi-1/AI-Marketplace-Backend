using AI_Marketplace.Domain.Entities;
using AI_Marketplace.Domain.enums;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AI_Marketplace.Application.Common.Interfaces
{
    public interface IPaymentRepository
    {
        // Create and Update
        Task<Domain.Entities.Payment> CreateAsync(Domain.Entities.Payment payment, CancellationToken cancellationToken = default);
        Task UpdateAsync(Domain.Entities.Payment payment, CancellationToken cancellationToken = default);
        
        // Retrieve
        Task<Domain.Entities.Payment?> GetByIdAsync(int paymentId, CancellationToken cancellationToken = default);
        Task<Domain.Entities.Payment?> GetByPaymentIntentIdAsync(string paymentIntentId, CancellationToken cancellationToken = default);
        Task<Domain.Entities.Payment?> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default);
        Task<List<Domain.Entities.Payment>> GetByOrderIdAsync(int orderId, CancellationToken cancellationToken = default);
        Task<List<Domain.Entities.Payment>> GetAllAsync(CancellationToken cancellationToken = default);
        
        // Queries by Status
        Task<List<Domain.Entities.Payment>> GetByStatusAsync(PaymentStatus status, CancellationToken cancellationToken = default);
        Task<List<Domain.Entities.Payment>> GetPendingPaymentsAsync(CancellationToken cancellationToken = default);
        Task<List<Domain.Entities.Payment>> GetFailedPaymentsAsync(CancellationToken cancellationToken = default);
        
        // Business Operations
        Task<bool> MarkAsSucceededAsync(int paymentId, string? transactionId, CancellationToken cancellationToken = default);
        Task<bool> MarkAsFailedAsync(int paymentId, string failureReason, CancellationToken cancellationToken = default);
        Task<bool> ProcessRefundAsync(int paymentId, long refundAmount, string? refundTransactionId, string? refundReason, CancellationToken cancellationToken = default);
    }
}
