using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Domain.Entities;
using AI_Marketplace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AI_Marketplace.Infrastructure.Repositories.RefreshTokens
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _context;

        public RefreshTokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken)
        {
            return await _context.Set<RefreshToken>()
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
        }

        public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
        {
            await _context.Set<RefreshToken>().AddAsync(refreshToken, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
        {
            _context.Set<RefreshToken>().Update(refreshToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoveOldTokensAsync(int userId, CancellationToken cancellationToken)
        {
            var tokens = await _context.Set<RefreshToken>()
                .Where(rt => rt.UserId == userId)
                .OrderByDescending(rt => rt.CreatedAt)
                .Skip(5) // Keep last 5 tokens
                .ToListAsync(cancellationToken);

            if (tokens.Any())
            {
                _context.Set<RefreshToken>().RemoveRange(tokens);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}