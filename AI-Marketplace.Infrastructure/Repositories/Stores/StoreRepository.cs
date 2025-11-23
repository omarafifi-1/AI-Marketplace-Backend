using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Domain.Entities;
using AI_Marketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using AI_Marketplace.Infrastructure.Repositories.Stores;

namespace AI_Marketplace.Infrastructure.Repositories.Stores
{
    public class StoreRepository : IStoreRepository
    {
        private readonly ApplicationDbContext _context;

        public StoreRepository( ApplicationDbContext context )
        {
            _context = context;
        }

        public async Task<Store> CreateAsync(Store store, CancellationToken cancellationToken = default)
        {
            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }
            _context.Stores.Add(store);
            await _context.SaveChangesAsync(cancellationToken);
            return store;
        }

        public async Task<Store?> GetByOwnerIdAsync(int ownerId, CancellationToken cancellationToken = default)
        {
            if (ownerId == 0)
            {
                throw new ArgumentException("Owner Id Cannot be Zero.", nameof(ownerId));
            }
            return await _context.Stores.FirstOrDefaultAsync(s => s.OwnerId == ownerId, cancellationToken);
        }

        public async Task<Store?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id == 0)
            {
                throw new ArgumentException("Id Cannot be Zero.", nameof(id));
            }
            return await _context.Stores.FindAsync([id], cancellationToken);
        }

        public async Task<bool> ExistsByOwnerIdAsync(int ownerId, CancellationToken cancellationToken = default)
        {
            return await _context.Stores.AnyAsync(s => s.OwnerId == ownerId, cancellationToken);
        }

        public async Task<Store> UpdateAsync(Store store, CancellationToken cancellationToken = default)
        {
            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }
            
            _context.Stores.Update(store);
            await _context.SaveChangesAsync(cancellationToken);
            return store;
        }
    }
}
