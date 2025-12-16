using AI_Marketplace.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AI_Marketplace.Application.Common.Interfaces
{
    public interface ICustomRequestRepository
    {
        Task<CustomRequest> CreateAsync(CustomRequest customRequest, CancellationToken cancellationToken);
        Task<List<CustomRequest>> GetByUserIdAsync (int userId, CancellationToken cancellationToken = default);
        Task<CustomRequest?> GetByIdAsync(int customRequestId, CancellationToken cancellationToken = default);
        Task<List<CustomRequest>> GetAllAsync(CancellationToken cancellationToken = default);
        Task DeleteAsync (int customRequestId, CancellationToken cancellationToken = default);
        Task UpdateAsync(CustomRequest customRequest, CancellationToken cancellationToken = default);
    }
}
