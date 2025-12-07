using AI_Marketplace.Application.Admin.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Admin.Queries
{
    public class GetPlatformStatsQuery : IRequest<StatsDto>
    {
        public GetPlatformStatsQuery() 
        {
        }
    }
}
