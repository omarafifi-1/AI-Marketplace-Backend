using MediatR;
using AI_Marketplace.Application.Carts.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Carts.Queries
{
    public record GetCartByUserIdQuery(int UserId) : IRequest<CartDto?>;


}
