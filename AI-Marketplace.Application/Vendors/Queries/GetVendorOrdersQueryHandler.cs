using AI_Marketplace.Application.Common.Exceptions;
using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Orders.DTOs;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Vendors.Queries
{
    public class GetVendorOrdersQueryHandler : IRequestHandler<GetVendorOrdersQuery, List<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly IStoreRepository _storeRepository;

        public GetVendorOrdersQueryHandler(IOrderRepository orderRepository, IMapper mapper, IStoreRepository storeRepository)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _storeRepository = storeRepository;
        }
        public async Task<List<OrderDto>> Handle(GetVendorOrdersQuery request, CancellationToken cancellationToken)
        {
            var store = await _storeRepository.GetByOwnerIdAsync(request.VendorId, cancellationToken);
            if (store == null)
            {
                throw new NotFoundException(new Dictionary<string, string[]>
                {
                    { "Store", new[] { "Store Not Found For The Given Vendor." } }
                });
            }
            var orders = await _orderRepository.GetOrdersByStoreId(store.Id, cancellationToken);
            var orderDtos = _mapper.Map<List<OrderDto>>(orders);
            return orderDtos;
        }
    }
}
