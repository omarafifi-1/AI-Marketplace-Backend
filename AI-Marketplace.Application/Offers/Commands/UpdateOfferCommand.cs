using AI_Marketplace.Application.Offers.DTOs;
using MediatR;

namespace AI_Marketplace.Application.Offers.Commands
{
    public class UpdateOfferCommand : IRequest<OfferResponseDto>
    {
        public int OfferId { get; set; }
        
        public int UserId { get; set; }
        
        public decimal ProposedPrice { get; set; }
        
        public int EstimatedDays { get; set; }
        
        public string? Message { get; set; }
    }
}
