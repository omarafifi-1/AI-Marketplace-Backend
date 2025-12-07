using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Orders.DTOs;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AI_Marketplace.Application.Orders.Queries
{
    public class GetOrdersByStoreIdQueryHandler : IRequestHandler<GetOrdersByStoreIdQuery, List<OrderDto?>>
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepo;

        public GetOrdersByStoreIdQueryHandler(IMapper mapper, IOrderRepository orderRepo)
        {
            _mapper = mapper;
            _orderRepo = orderRepo;
        }

        public async Task<List<OrderDto?>> Handle(GetOrdersByStoreIdQuery request, CancellationToken cancellationToken)
        {
            var orders = await _orderRepo.GetOrdersByStoreIdAsync(request.storeId, cancellationToken);
            var orderDtos = _mapper.Map<List<OrderDto?>>(orders);
            return orderDtos;
        }
    }
}
