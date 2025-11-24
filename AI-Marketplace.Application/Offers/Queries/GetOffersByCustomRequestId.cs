using AI_Marketplace.Application.Offers.DTOs;
using MediatR;

namespace AI_Marketplace.Application.Offers.Queries.GetOffersByCustomRequestId
{
  
    public class GetOffersByCustomRequestIdQuery : IRequest<List<OfferResponseDto>>
    {
        public int CustomRequestId { get; set; }
    }
}