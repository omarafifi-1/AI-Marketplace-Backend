using AI_Marketplace.Application.Orders.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Orders.Queries
{
    public record GetAllOrdersForUserQuery(int UserId): IRequest<List<OrderDto>>;
}
