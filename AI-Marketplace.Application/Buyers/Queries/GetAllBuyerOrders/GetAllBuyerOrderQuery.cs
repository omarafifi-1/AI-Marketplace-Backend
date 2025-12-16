using AI_Marketplace.Application.Orders.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Buyers.Queries.GetAllBuyerOrders
{
    public class GetAllBuyerOrderQuery : IRequest<IEnumerable<OrderDto>>
    {
        public int BuyerId { get; set; }

        public string? Status { get; set; }


    }
}
