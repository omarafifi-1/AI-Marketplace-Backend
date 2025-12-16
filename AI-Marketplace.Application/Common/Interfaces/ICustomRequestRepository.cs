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
        Task<CustomRequest?> GetByIdAsync(int customRequestId, CancellationToken cancellationToken = default);
        
        Task UpdateAsync(CustomRequest customRequest, CancellationToken cancellationToken = default);
    }
}
