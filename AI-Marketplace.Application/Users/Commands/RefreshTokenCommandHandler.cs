using AI_Marketplace.Application.Common.Exceptions;
using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Users.DTOs;
using AI_Marketplace.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AI_Marketplace.Application.Users.Commands
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, LoginResponseDto>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IStoreRepository _storeRepository;

        public RefreshTokenCommandHandler(
            UserManager<ApplicationUser> userManager,
            IJwtTokenService jwtTokenService,
            IRefreshTokenRepository refreshTokenRepository,
            IStoreRepository storeRepository)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
            _refreshTokenRepository = refreshTokenRepository;
            _storeRepository = storeRepository;
        }

        public async Task<LoginResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var refreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);

            if (refreshToken == null || !refreshToken.IsActive)
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "RefreshToken", new[] { "Invalid or expired refresh token." } }
                });
            }

            var user = await _userManager.FindByIdAsync(refreshToken.UserId.ToString());
            if (user == null || !user.IsActive)
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "User", new[] { "User not found or inactive." } }
                });
            }

            // Revoke old refresh token
            refreshToken.RevokedAt = DateTime.UtcNow;
            refreshToken.RevokedByIp = request.IpAddress;

            // Generate new tokens
            var roles = await _userManager.GetRolesAsync(user);
            var newAccessToken = _jwtTokenService.GenerateAccessToken(user, roles);
            var newRefreshToken = _jwtTokenService.GenerateRefreshToken(user.Id, request.IpAddress);

            // Link old token to new one
            refreshToken.ReplacedByToken = newRefreshToken.Token;
            await _refreshTokenRepository.UpdateAsync(refreshToken, cancellationToken);

            // Save new refresh token
            await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);

            // Remove old tokens (keep only last 5)
            await _refreshTokenRepository.RemoveOldTokensAsync(user.Id, cancellationToken);

            // Get store info if seller
            Store? store = null;
            if (roles.Contains("Seller"))
            {
                store = await _storeRepository.GetByOwnerIdAsync(user.Id, cancellationToken);
            }

            return new LoginResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token,
                AccessTokenExpiration = DateTime.UtcNow.AddMinutes(15),
                RefreshTokenExpiration = newRefreshToken.ExpiresAt,
                User = new UserResponseDto
                {
                    Id = user.Id,
                    UserName = user.UserName!,
                    Email = user.Email!,
                    Role = roles.FirstOrDefault() ?? string.Empty,
                    CreatedAt = user.CreatedAt,
                    StoreId = store?.Id,
                    StoreName = store?.StoreName
                }
            };
        }
    }
}
