using AI_Marketplace.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace AI_Marketplace.Application.Common.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken);
        Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
        Task UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
        Task RemoveOldTokensAsync(int userId, CancellationToken cancellationToken);
    }
}