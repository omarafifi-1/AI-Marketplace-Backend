using AI_Marketplace.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Common.Interfaces
{
    public interface IProductRepository
    {
        IQueryable<Product> GetQueryable();      
        
        Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken);

        Task<Product> CreateAsync(Product product, CancellationToken cancellationToken);

        Task<ProductImage> UploadProductImageAsync(ProductImage productImage, CancellationToken cancellationToken);

        Task UpdateAsync(Product product, CancellationToken cancellationToken);

        Task DeleteAsync(Product product, CancellationToken cancellationToken);
    }
}
