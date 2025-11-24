using AI_Marketplace.Application.Offers.DTOs;
using MediatR;

namespace AI_Marketplace.Application.Offers.Queries.GetOffersByStoreId
{
    public class GetOffersByStoreIdQuery : IRequest<PagedOfferDto>
    {
        public int UserId { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }
}