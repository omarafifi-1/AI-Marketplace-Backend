using AI_Marketplace.Application.Categories.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Categories.Commands
{
    public class UpdateCategoryCommand : IRequest<GetCategoryDto>
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? ParentCategoryId { get; set; }
        public bool RemoveParentCategory { get; set; } = false;
    }
}
