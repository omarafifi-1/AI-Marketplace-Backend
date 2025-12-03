using AI_Marketplace.Application.Buyers.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Buyers.Queries.GetBuyerStats
{
    public class GetBuyerStatsQuery : IRequest<BuyerStatsDto>
    {
        public int BuyerId { get; }

        public GetBuyerStatsQuery(int buyerId)
        {
            BuyerId = buyerId;
        }
    }
}
