using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Products.DTOs
{
    public class PagedProductDto
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public List<GetProductDto> Items { get; set; } = new();
    }
}
