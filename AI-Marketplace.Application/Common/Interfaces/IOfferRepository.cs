using AI_Marketplace.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AI_Marketplace.Application.Common.Interfaces
{
    public interface IOfferRepository
    {
        Task<Offer> AddAsync(Offer offer, CancellationToken cancellationToken = default);

        Task<List<Offer>> GetByCustomRequestIdAsync(int customRequestId, CancellationToken cancellationToken = default);

        Task<(int TotalCount, List<Offer> Offers)> GetByStoreIdAsync(int storeId, int page, int pageSize, CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(int customRequestId, int storeId, CancellationToken cancellationToken = default);

        Task<Offer?> GetByIdAsync(int offerId, CancellationToken cancellationToken = default);

        Task UpdateAsync(Offer offer, CancellationToken cancellationToken = default);

        Task<List<Offer>> GetPendingByCustomRequestIdAsync(int customRequestId, CancellationToken cancellationToken = default);
    }
}
