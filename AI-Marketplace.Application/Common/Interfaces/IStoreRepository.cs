using AI_Marketplace.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Common.Interfaces
{
    public interface IStoreRepository
    {
        public Task<Store> CreateAsync(Store store, CancellationToken cancellationToken = default);
        public Task<Store?> GetByOwnerIdAsync(int ownerId, CancellationToken cancellationToken = default);
        public Task<Store?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        public Task<bool> ExistsByOwnerIdAsync(int ownerId, CancellationToken cancellationToken = default);
    }
}
