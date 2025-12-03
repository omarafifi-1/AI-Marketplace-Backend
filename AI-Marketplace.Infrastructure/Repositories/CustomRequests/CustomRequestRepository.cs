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

        public async Task<CustomRequest?> GetByIdAsync(int customRequestId, CancellationToken cancellationToken = default)
        {
            return await _context.CustomRequests
                .Include(cr => cr.Buyer)
                .Include(cr => cr.Category)
                .Include(cr => cr.Offers)
                .Include(cr => cr.GeneratedImages)
                .FirstOrDefaultAsync(cr => cr.Id == customRequestId, cancellationToken);
        }

        public async Task UpdateAsync(CustomRequest customRequest, CancellationToken cancellationToken = default)
        {
            // Use ExecuteUpdateAsync for direct SQL UPDATE - no tracking issues
            var rowsAffected = await _context.CustomRequests
                .Where(cr => cr.Id == customRequest.Id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(cr => cr.Status, customRequest.Status)
                    .SetProperty(cr => cr.UpdatedAt, customRequest.UpdatedAt)
                    .SetProperty(cr => cr.Description, customRequest.Description),
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