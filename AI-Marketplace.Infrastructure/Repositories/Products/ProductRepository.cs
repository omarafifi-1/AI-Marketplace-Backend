using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Domain.Entities;
using AI_Marketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Infrastructure.Repositories.Products
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository( ApplicationDbContext context)
        {
            _context = context;
            
        }
        public async Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Products
               .Include(p => p.ProductImages)
               .Include(p => p.Store)
               .Include(p => p.Category)
               .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public IQueryable<Product> GetQueryable()
        {
            return _context.Products.AsQueryable();
            
        }

        public async Task<Product> CreateAsync(Product product, CancellationToken cancellationToken)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync(cancellationToken);
            return product;
        }

        public async Task<ProductImage> UploadProductImageAsync(ProductImage productImage, CancellationToken cancellationToken)
        {
            _context.ProductImages.Add(productImage);
            await _context.SaveChangesAsync(cancellationToken);
            return productImage;
        }

        public async Task UpdateAsync(Product product, CancellationToken cancellationToken)
        {
            _context.Products.Update(product);
            await  _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Product product, CancellationToken cancellationToken)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteProductImageAsync(ProductImage productImage, CancellationToken cancellationToken)
        {
            _context.ProductImages.Remove(productImage);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
