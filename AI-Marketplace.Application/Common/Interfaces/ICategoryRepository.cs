using AI_Marketplace.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Common.Interfaces
{
    public interface ICategoryRepository
    {
        public Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        public Task<List<Category>> GetAllAsync(CancellationToken cancellationToken = default);
        public Task<Category> CreateAsync(Category category, CancellationToken cancellationToken = default);
    }
}
