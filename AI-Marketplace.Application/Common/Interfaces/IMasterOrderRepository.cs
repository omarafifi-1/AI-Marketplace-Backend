using AI_Marketplace.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace AI_Marketplace.Application.Common.Interfaces
{
    public interface IMasterOrderRepository
    {
        Task<MasterOrder> CreateAsync(MasterOrder masterOrder, CancellationToken cancellationToken = default);
        Task<MasterOrder?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<MasterOrder?> GetByIdWithChildOrdersAsync(int id, CancellationToken cancellationToken = default);
        Task UpdateAsync(MasterOrder masterOrder, CancellationToken cancellationToken = default);
    }
}
