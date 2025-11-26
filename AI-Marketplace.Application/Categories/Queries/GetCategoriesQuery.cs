using AI_Marketplace.Application.Categories.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Categories.Queries
{
    public class GetCategoriesQuery : IRequest<List<GetCategoriesDto>>
    {
    }
}
