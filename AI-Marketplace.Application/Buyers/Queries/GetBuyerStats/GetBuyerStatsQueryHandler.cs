using AI_Marketplace.Application.Buyers.DTOs;
using AI_Marketplace.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using AI_Marketplace.Domain.enums;

namespace AI_Marketplace.Application.Buyers.Queries.GetBuyerStats
{
    public class GetBuyerStatsQueryHandler : IRequestHandler<GetBuyerStatsQuery, BuyerStatsDto>
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IMapper _mapper;

        public GetBuyerStatsQueryHandler(IOrderRepository orderRepo, IMapper mapper)
        {
            _orderRepo = orderRepo;
            _mapper = mapper;
        }
        public async Task<BuyerStatsDto> Handle(GetBuyerStatsQuery request, CancellationToken cancellationToken)
        {
            var orders = await _orderRepo.GetOrdersByBuyerIdAsync(request.BuyerId, cancellationToken);

            var stats = new BuyerStatsDto
            {
                TotalOrders = orders.Count,
                PendingOrders = orders.Count(o => o.Status == OrderStateTypes.Pending.ToString()),
                CompletedOrders = orders.Count(o => o.Status == OrderStateTypes.Delivered.ToString()),
                TotalSpent = orders.Sum(o => o.TotalAmount)
            };

            return stats;
        }
    }
}
