using AI_Marketplace.Application.Products.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Products.Commands
{
    public class CreateProductCommand : IRequest<GetProductDto>
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; } = 0;
        public int CategoryId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
