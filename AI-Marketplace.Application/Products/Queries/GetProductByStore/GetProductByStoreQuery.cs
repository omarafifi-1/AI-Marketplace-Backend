using AI_Marketplace.Application.Products.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Products.Queries.GetProductByStore
{
    public class GetProductByStoreQuery : IRequest<List<GetProductDto>>
    {
        public int UserId { get; set;}
    }
}
