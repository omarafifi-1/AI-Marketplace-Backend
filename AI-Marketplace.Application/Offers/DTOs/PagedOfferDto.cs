using System.Collections.Generic;

namespace AI_Marketplace.Application.Offers.DTOs
{
    public class PagedOfferDto
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public List<OfferResponseDto> Items { get; set; } = new();
    }
}