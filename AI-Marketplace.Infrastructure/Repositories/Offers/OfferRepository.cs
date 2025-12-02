using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Common.Exceptions;
using AI_Marketplace.Domain.Entities;
using AI_Marketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AI_Marketplace.Infrastructure.Repositories.Offers
{
    public class OfferRepository : IOfferRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OfferRepository> _logger;

        public OfferRepository(ApplicationDbContext context, ILogger<OfferRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Offer> AddAsync(Offer offer, CancellationToken cancellationToken = default)
        {
            try
            {
                var exists = await ExistsAsync(offer.CustomRequestId, offer.StoreId, cancellationToken);
                if (exists)
                {
                    _logger.LogWarning(
                        "Duplicate offer attempt: StoreId={StoreId}, CustomRequestId={CustomRequestId}",
                        offer.StoreId,
                        offer.CustomRequestId);
                    throw new DuplicateOfferException(offer.StoreId, offer.CustomRequestId);
                }

                _context.Offers.Add(offer);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Offer created successfully: OfferId={OfferId}, StoreId={StoreId}, CustomRequestId={CustomRequestId}",
                    offer.Id,
                    offer.StoreId,
                    offer.CustomRequestId);

                return offer;
            }
            catch (DbUpdateException ex) when (
                ex.InnerException?.Message.Contains("IX_Offers_CustomRequestId_StoreId") == true ||
                ex.InnerException?.Message.Contains("unique constraint", StringComparison.OrdinalIgnoreCase) == true)
            {
                _logger.LogWarning(
                    ex,
                    "Database unique constraint violation: StoreId={StoreId}, CustomRequestId={CustomRequestId}",
                    offer.StoreId,
                    offer.CustomRequestId);
                throw new DuplicateOfferException(offer.StoreId, offer.CustomRequestId);
            }
        }

        public async Task<List<Offer>> GetByCustomRequestIdAsync(int customRequestId, CancellationToken cancellationToken = default)
        {
            var offers = await _context.Offers
                .AsNoTracking()
                .Include(o => o.Store)
                .Where(o => o.CustomRequestId == customRequestId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync(cancellationToken);

            _logger.LogInformation(
                "Retrieved {Count} offers for CustomRequestId={CustomRequestId}",
                offers.Count,
                customRequestId);

            return offers;
        }

        public async Task<(int TotalCount, List<Offer> Offers)> GetByStoreIdAsync(
            int storeId,
            int page = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _context.Offers
                .AsNoTracking()
                .Include(o => o.CustomRequest)
                .Include(o => o.Store)
                .Where(o => o.StoreId == storeId);

            var totalCount = await query.CountAsync(cancellationToken);

            var offers = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            _logger.LogInformation(
                "Retrieved page {Page} of offers for StoreId={StoreId} (PageSize={PageSize}, TotalCount={TotalCount})",
                page,
                storeId,
                pageSize,
                totalCount);

            return (totalCount, offers);
        }

        public async Task<bool> ExistsAsync(int customRequestId, int storeId, CancellationToken cancellationToken = default)
        {
            return await _context.Offers
                .AsNoTracking()
                .AnyAsync(o => o.CustomRequestId == customRequestId && o.StoreId == storeId, cancellationToken);
        }

        public async Task<Offer?> GetByIdAsync(int offerId, CancellationToken cancellationToken = default)
        {
            var offer = await _context.Offers
                .AsNoTracking()
                .Include(o => o.Store)
                .Include(o => o.CustomRequest)
                .FirstOrDefaultAsync(o => o.Id == offerId, cancellationToken);

            if (offer != null)
            {
                _logger.LogInformation(
                    "Retrieved offer: OfferId={OfferId}, StoreId={StoreId}, CustomRequestId={CustomRequestId}, Status={Status}",
                    offer.Id,
                    offer.StoreId,
                    offer.CustomRequestId,
                    offer.Status);
            }
            else
            {
                _logger.LogWarning("Offer not found: OfferId={OfferId}", offerId);
            }

            return offer;
        }

        public async Task UpdateAsync(Offer offer, CancellationToken cancellationToken = default)
        {
            try
            {
                _context.Offers.Update(offer);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Offer updated successfully: OfferId={OfferId}, StoreId={StoreId}, Status={Status}, Price={Price}, Days={Days}",
                    offer.Id,
                    offer.StoreId,
                    offer.Status,
                    offer.ProposedPrice,
                    offer.EstimatedDays);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(
                    ex,
                    "Concurrency error updating offer: OfferId={OfferId}",
                    offer.Id);
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Database error updating offer: OfferId={OfferId}",
                    offer.Id);
                throw;
            }
        }

        public async Task<List<Offer>> GetPendingByCustomRequestIdAsync(int customRequestId, CancellationToken cancellationToken = default)
        {
            var offers = await _context.Offers
                .AsNoTracking()
                .Include(o => o.Store)
                .Where(o => o.CustomRequestId == customRequestId && o.Status == "Pending")
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync(cancellationToken);

            _logger.LogInformation(
                "Retrieved {Count} pending offers for CustomRequestId={CustomRequestId}",
                offers.Count,
                customRequestId);

            return offers;
        }
    }
}
