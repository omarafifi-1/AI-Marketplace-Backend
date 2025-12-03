using AI_Marketplace.Application.Offers.DTOs;
using MediatR;

namespace AI_Marketplace.Application.Offers.Commands
{
    public class AcceptOfferCommand : IRequest<OrderResponseDto>
    {
        public int OfferId { get; set; }
        
        public int UserId { get; set; }
        
        public string ShippingAddress { get; set; } = string.Empty;
    }
}
