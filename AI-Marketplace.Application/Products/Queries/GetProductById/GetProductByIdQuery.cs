using AI_Marketplace.Application.Products.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Products.Queries.GetProductById
{
    public class GetProductByIdQuery : IRequest<GetProductDto?>
    {
        public int Id { get; set; }
    }
}
