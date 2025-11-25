using AI_Marketplace.Application.Products.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Products.Queries.FilteredProducts
{
    public class GetFilteredProductsQuery : IRequest<IEnumerable<GetProductDto>>
    {
        public int? CategoryId { get; set; }

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        public string? Keyword { get; set; }
    }
}
