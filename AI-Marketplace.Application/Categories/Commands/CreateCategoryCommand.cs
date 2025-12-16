using AI_Marketplace.Application.Categories.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Categories.Commands
{
    public class CreateCategoryCommand : IRequest<GetCategoryDto>
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? ParentCategoryId { get; set; }
    }
}
