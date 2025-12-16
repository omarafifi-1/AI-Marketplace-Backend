using AI_Marketplace.Application.Common.Exceptions;
using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Domain.Entities;
using AI_Marketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AI_Marketplace.Infrastructure.Repositories.CustomRequests
{
    public class CustomRequestRepository : ICustomRequestRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomRequestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CustomRequest> CreateAsync(CustomRequest customRequest, CancellationToken cancellationToken)
        {
            if (customRequest == null)
                throw new ArgumentNullException(nameof(customRequest));

            _context.CustomRequests.Add(customRequest);
            await _context.SaveChangesAsync(cancellationToken);
            
            return customRequest; 
        }

        public async Task DeleteAsync(int customRequestId, CancellationToken cancellationToken = default)
        {
            var customRequest = await _context.CustomRequests
                .FirstOrDefaultAsync(cr => cr.Id == customRequestId, cancellationToken);
                
            if (customRequest == null)
            {
                throw new NotFoundException(new Dictionary<string, string[]>
                {
                    { "CustomRequest", new[] { $"Custom request with ID {customRequestId} not found." } }
                });
            }
            
            _context.CustomRequests.Remove(customRequest);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public Task<List<CustomRequest>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return _context.CustomRequests
                .Include(cr => cr.Buyer)
                .Include(cr => cr.Category)
                .Include(cr => cr.Offers)
                .Include(cr => cr.GeneratedImages)
                .ToListAsync(cancellationToken);
        }

        public async Task<CustomRequest?> GetByIdAsync(int customRequestId, CancellationToken cancellationToken = default)
        {
            return await _context.CustomRequests
                .Include(cr => cr.Buyer)
                .Include(cr => cr.Category)
                .Include(cr => cr.Offers)
                .Include(cr => cr.GeneratedImages)
                .FirstOrDefaultAsync(cr => cr.Id == customRequestId, cancellationToken);
        }

        public Task<List<CustomRequest>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return _context.CustomRequests
                .Where(cr => cr.BuyerId == userId)
                .Include(cr => cr.Buyer)
                .Include(cr => cr.Category)
                .Include(cr => cr.Offers)
                .Include(cr => cr.GeneratedImages)
                .ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(CustomRequest customRequest, CancellationToken cancellationToken = default)
        {
            if (customRequest == null)
                throw new ArgumentNullException(nameof(customRequest));

            var rowsAffected = await _context.CustomRequests
                .Where(cr => cr.Id == customRequest.Id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(cr => cr.Status, customRequest.Status)
                    .SetProperty(cr => cr.UpdatedAt, customRequest.UpdatedAt)
                    .SetProperty(cr => cr.Description, customRequest.Description)
                    .SetProperty(cr => cr.CategoryId, customRequest.CategoryId)
                    .SetProperty(cr => cr.ImageUrl, customRequest.ImageUrl)
                    .SetProperty(cr => cr.Budget, customRequest.Budget)
                    .SetProperty(cr => cr.Deadline, customRequest.Deadline),
            cancellationToken);
           

            if (rowsAffected == 0)
            {
                throw new NotFoundException(new Dictionary<string, string[]>
                {
                    { "CustomRequest", new[] { $"Custom request with ID {customRequest.Id} not found." } }
                });
            }
        }
    }
}