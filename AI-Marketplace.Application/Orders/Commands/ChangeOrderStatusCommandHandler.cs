using AI_Marketplace.Application.Common.Exceptions;
using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Orders.DTOs;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Orders.Commands
{
    public class ChangeOrderStatusCommandHandler : IRequestHandler<ChangeOrderStatusCommand, OrderDto>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IMapper _mapper;

        public ChangeOrderStatusCommandHandler(IOrderRepository orderRepository, IStoreRepository storeRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _storeRepository = storeRepository;
            _mapper = mapper;
        }
        public async Task<OrderDto> Handle(ChangeOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var store = await _storeRepository.GetByOwnerIdAsync(request.UserId, cancellationToken);
            if (store == null)
            {
                throw new NotFoundException(new Dictionary<string, string[]>
                {
                    { "Store", new[] { "Store Not Found For The Given User." } }
                });
            }
            var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
            if (order == null || order.StoreId != store.Id)
            {
                throw new NotFoundException(new Dictionary<string, string[]>
                {
                    { "Order", new[] { "Order Not Found For The Given Store." } }
                });
            }
            List<string> validStatuses = new List<string> { "Pending", "Processing", "Shipped", "Delivered", "Cancelled" };
            if (!validStatuses.Contains(request.Status))
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Status", new[] { "Invalid Order Status." } }
                });
            }
            order.Status = request.Status;
            await _orderRepository.UpdateOrderAsync(order, cancellationToken);
            return _mapper.Map<OrderDto>(order);
        }
    }
}
