using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Orders.DTOs;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Orders.Queries
{
    public class GetAllOrdersForUserQueryHandler : IRequestHandler<GetAllOrdersForUserQuery, List<OrderDto?>>
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepo;

        public GetAllOrdersForUserQueryHandler(IMapper mapper , IOrderRepository orderRepo ) {
            _mapper = mapper;
            _orderRepo = orderRepo;
        }
        public async Task<List<OrderDto?>> Handle(GetAllOrdersForUserQuery request, CancellationToken cancellationToken)
        {
            var userOrders = await _orderRepo.GetOrdersByBuyerIdAsync(request.UserId, cancellationToken);
            if (userOrders == null)
            {
                return null;
            }
            var mappedOrders = _mapper.Map<List<OrderDto>>(userOrders);
            return mappedOrders.Cast<OrderDto?>().ToList();
        }
    }
}
