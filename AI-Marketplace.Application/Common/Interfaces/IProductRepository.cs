using AI_Marketplace.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Common.Interfaces
{
    public interface IProductRepository
    {
        IQueryable<Product> GetQueryable();        
        Task<Product?> GetByIdAsync(int id);
    }
}
