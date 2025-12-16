using AI_Marketplace.Application.Carts.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Carts.Queries
{
    //If a class is only carrying data and it's not likely that data will be mutated later , we should use a record

    public record GetCartByIdQuery(int Id) : IRequest<CartDto?>;
}
