using AI_Marketplace.Application.Orders.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Orders.Queries
{
    public record GetAllOrdersQuery :  IRequest<List<OrderDto?>>;

}
