using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Orders.DTOs;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Buyers.Queries.GetAllBuyerOrders
{
    public class GetAllBuyerOrderQueryHandler : IRequestHandler<GetAllBuyerOrderQuery, IEnumerable<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public GetAllBuyerOrderQueryHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;

        }
        public async Task<IEnumerable<OrderDto>> Handle(GetAllBuyerOrderQuery request, CancellationToken cancellationToken)
        {
            // Fetch all orders with includes
            var allOrders = await _orderRepository.GetAllOrdersAsync(cancellationToken);

            // Filter by buyer
            var buyerOrders = allOrders.AsQueryable().Where(o => o.BuyerId == request.BuyerId);

            // Status filter if provided
            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                var status = request.Status.Trim().ToLowerInvariant();
                switch (status)
                {
                    case "pending":
                        buyerOrders = buyerOrders.Where(o => o.Status.ToLower() == "pending");
                        break;
                    case "completed":
                        buyerOrders = buyerOrders.Where(o => o.Status.ToLower() == "delivered");
                        break;
                    case "all":
                    default:
                        break;
                }
            }

            var list = buyerOrders
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            // Map to DTO
            return _mapper.Map<IEnumerable<OrderDto>>(list);
        }
    }
}
