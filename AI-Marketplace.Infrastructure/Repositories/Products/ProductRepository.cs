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
        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
               .Include(p => p.ProductImages)
               .Include(p => p.Store)
               .Include(p => p.Category)
               .FirstOrDefaultAsync(p => p.Id == id);
        }

        public IQueryable<Product> GetQueryable()
        {
            return _context.Products.AsQueryable();
            
        }
    }
}
