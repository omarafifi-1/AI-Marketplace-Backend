using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Orders.DTOs;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AI_Marketplace.Application.Orders.Queries
{
    public record GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, List<OrderDto?>>
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepo;

        public GetAllOrdersQueryHandler(IMapper mapper, IOrderRepository orderRepo)
        {
            _mapper = mapper;
            _orderRepo = orderRepo;
        }

        public async Task<List<OrderDto?>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            var allOrders = await _orderRepo.GetAllOrdersAsync(cancellationToken);
            var orderDtos = _mapper.Map<List<OrderDto?>>(allOrders);
            return orderDtos;
        }
    }
}
