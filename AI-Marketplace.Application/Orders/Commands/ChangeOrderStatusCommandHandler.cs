using AI_Marketplace.Application.Common.Exceptions;
using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Orders.DTOs;
using AI_Marketplace.Domain.enums;
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
        private readonly ICustomRequestRepository _customRequestRepository;
        private readonly IOfferRepository _offerRepository;
        private readonly IMapper _mapper;

        public ChangeOrderStatusCommandHandler(
            IOrderRepository orderRepository, 
            IStoreRepository storeRepository,
            ICustomRequestRepository customRequestRepository,
            IOfferRepository offerRepository,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _storeRepository = storeRepository;
            _customRequestRepository = customRequestRepository;
            _offerRepository = offerRepository;
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
            var order = await _orderRepository.GetOrderByIdAsync(request.OrderId, cancellationToken);
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

            // If order is delivered and has an associated offer, mark the custom request as completed
            if (request.Status == "Delivered" && order.OfferId.HasValue)
            {
                var offer = await _offerRepository.GetByIdAsync(order.OfferId.Value, cancellationToken);
                if (offer != null && offer.CustomRequestId > 0)
                {
                    var customRequest = await _customRequestRepository.GetByIdAsync(offer.CustomRequestId, cancellationToken);
                    if (customRequest != null && customRequest.Status != CustomRequestStatus.Completed)
                    {
                        customRequest.Status = CustomRequestStatus.Completed;
                        customRequest.UpdatedAt = DateTime.UtcNow;
                        await _customRequestRepository.UpdateAsync(customRequest, cancellationToken);
                    }
                }
            }

            return _mapper.Map<OrderDto>(order);
        }
    }
}
