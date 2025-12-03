using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Orders.DTOs;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Buyers.Queries.GetBuyerOrderById
{
    internal class GetBuyerOrderByIdQueryHandler : IRequestHandler<GetBuyerOrderByIdQuery, OrderDto?>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public GetBuyerOrderByIdQueryHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }
        public async Task<OrderDto?> Handle(GetBuyerOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

            if (order == null) return null;

            // ensure buyer only accesses their own order
            if (order.BuyerId != request.BuyerId) return null;

            return _mapper.Map<OrderDto>(order);
        }
    }
}
