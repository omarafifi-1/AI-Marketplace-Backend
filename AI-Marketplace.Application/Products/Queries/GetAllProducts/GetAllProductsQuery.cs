using AI_Marketplace.Application.Products.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Products.Queries.GetAllProducts
{
    public record GetAllProductsQuery(
        int Page = 1,
        int PageSize = 20,
        string? SortBy = "date",          
        string? SortDirection = "desc"
        ) : IRequest<PagedProductDto>;
    
}
