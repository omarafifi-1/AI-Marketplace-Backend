using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Domain.Entities;
using AI_Marketplace.Domain.enums;
using AI_Marketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AI_Marketplace.Infrastructure.Repositories.Payments
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Domain.Entities.Payment> CreateAsync(Domain.Entities.Payment payment, CancellationToken cancellationToken = default)
        {
            await _context.Payments.AddAsync(payment, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return payment;
        }

        public async Task UpdateAsync(Domain.Entities.Payment payment, CancellationToken cancellationToken = default)
        {
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<Domain.Entities.Payment?> GetByIdAsync(int paymentId, CancellationToken cancellationToken = default)
        {
            return await _context.Payments
                .Include(p => p.Order)
                    .ThenInclude(o => o.Buyer)
                .Include(p => p.Order)
                    .ThenInclude(o => o.Store)
                .FirstOrDefaultAsync(p => p.Id == paymentId, cancellationToken);
        }

        public async Task<Domain.Entities.Payment?> GetByPaymentIntentIdAsync(string paymentIntentId, CancellationToken cancellationToken = default)
        {
            return await _context.Payments
                .Include(p => p.Order)
                    .ThenInclude(o => o.Buyer)
                .Include(p => p.Order)
                    .ThenInclude(o => o.Store)
                .FirstOrDefaultAsync(p => p.PaymentIntentId == paymentIntentId, cancellationToken);
        }

        public async Task<Domain.Entities.Payment?> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default)
        {
            return await _context.Payments
                .Include(p => p.Order)
                    .ThenInclude(o => o.Buyer)
                .Include(p => p.Order)
                    .ThenInclude(o => o.Store)
                .FirstOrDefaultAsync(p => p.TransactionId == transactionId, cancellationToken);
        }

        public async Task<List<Domain.Entities.Payment>> GetByOrderIdAsync(int orderId, CancellationToken cancellationToken = default)
        {
            return await _context.Payments
                .Include(p => p.Order)
                .Where(p => p.OrderId == orderId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Domain.Entities.Payment>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Payments
                .Include(p => p.Order)
                    .ThenInclude(o => o.Buyer)
                .Include(p => p.Order)
                    .ThenInclude(o => o.Store)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Domain.Entities.Payment>> GetByStatusAsync(PaymentStatus status, CancellationToken cancellationToken = default)
        {
            return await _context.Payments
                .Include(p => p.Order)
                    .ThenInclude(o => o.Buyer)
                .Where(p => p.Status == status)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Domain.Entities.Payment>> GetPendingPaymentsAsync(CancellationToken cancellationToken = default)
        {
            return await GetByStatusAsync(PaymentStatus.Pending, cancellationToken);
        }

        public async Task<List<Domain.Entities.Payment>> GetFailedPaymentsAsync(CancellationToken cancellationToken = default)
        {
            return await GetByStatusAsync(PaymentStatus.Failed, cancellationToken);
        }

        public async Task<bool> MarkAsSucceededAsync(int paymentId, string? transactionId, CancellationToken cancellationToken = default)
        {
            var payment = await _context.Payments.FindAsync([paymentId], cancellationToken);
            if (payment == null) return false;

            payment.Status = PaymentStatus.Succeeded;
            payment.ProcessedAt = DateTime.UtcNow;
            payment.CompletedAt = DateTime.UtcNow;
            
            if (!string.IsNullOrEmpty(transactionId))
            {
                payment.TransactionId = transactionId;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> MarkAsFailedAsync(int paymentId, string failureReason, CancellationToken cancellationToken = default)
        {
            var payment = await _context.Payments.FindAsync([paymentId], cancellationToken);
            if (payment == null) return false;

            payment.Status = PaymentStatus.Failed;
            payment.ProcessedAt = DateTime.UtcNow;
            payment.FailureReason = failureReason;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> ProcessRefundAsync(int paymentId, long refundAmount, string? refundTransactionId, string? refundReason, CancellationToken cancellationToken = default)
        {
            var payment = await _context.Payments.FindAsync([paymentId], cancellationToken);
            if (payment == null || payment.Status != PaymentStatus.Succeeded) return false;

            var totalRefunded = (payment.RefundedAmount ?? 0) + refundAmount;
            
            payment.RefundedAmount = totalRefunded;
            payment.RefundTransactionId = refundTransactionId;
            payment.RefundedAt = DateTime.UtcNow;
            payment.RefundReason = refundReason;
            
            // Determine if fully or partially refunded
            if (totalRefunded >= payment.Amount)
            {
                payment.Status = PaymentStatus.Refunded;
            }
            else
            {
                payment.Status = PaymentStatus.PartiallyRefunded;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
