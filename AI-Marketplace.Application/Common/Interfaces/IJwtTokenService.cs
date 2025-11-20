using AI_Marketplace.Domain.Entities;
using System.Collections.Generic;

namespace AI_Marketplace.Application.Common.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(ApplicationUser user, IList<string> roles);
    }
}