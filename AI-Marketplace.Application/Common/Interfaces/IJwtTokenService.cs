using AI_Marketplace.Domain.Entities;
using Microsoft.IdentityModel.Tokens.Experimental;
using System.Collections.Generic;
using System.Security.Claims;

namespace AI_Marketplace.Application.Common.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(ApplicationUser user, IList<string> roles);
        RefreshToken GenerateRefreshToken(int userId, string ipAddress);
        string GenerateAccessToken(ApplicationUser user, IList<string> roles);
        ClaimsPrincipal? ValidatedToken(string token);
    }
}