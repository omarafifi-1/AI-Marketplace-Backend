using AI_Marketplace.Application.Orders.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Buyers.Queries.GetBuyerOrderById
{
    public class GetBuyerOrderByIdQuery : IRequest<OrderDto?>
    {
        public int OrderId { get; }
        public int BuyerId { get; }

        public GetBuyerOrderByIdQuery(int orderId, int buyerId)
        {
            OrderId = orderId;
            BuyerId = buyerId;
        }
    }
}
