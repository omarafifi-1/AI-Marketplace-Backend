using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Categories.Commands
{
    public class DeleteCategoryCommand : IRequest<string>
    {
        public int Id { get; set; }
        public DeleteCategoryCommand(int id)
        {
            Id = id;
        }
    }
}
