using AI_Marketplace.Application.Carts.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Carts.Commands
{
    public record UpdateCartQuantityCommand(int UserId , int ProdId, int Quantity) : IRequest<CartDto?>;
}
