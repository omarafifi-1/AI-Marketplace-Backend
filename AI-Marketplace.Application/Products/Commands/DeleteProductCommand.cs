using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Products.Commands
{
    public class DeleteProductCommand : IRequest<string>
    {
        public int ProductId { get; set; }
        public int UserId { get; set; }
    }
}