using AI_Marketplace.Application.Offers.DTOs;
using MediatR;

namespace AI_Marketplace.Application.Offers.Queries
{
    public class GetOfferByIdQuery : IRequest<OfferResponseDto?>
    {
        public int OfferId { get; set; }
    }
}
