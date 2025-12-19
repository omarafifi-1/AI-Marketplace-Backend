using MediatR;

namespace AI_Marketplace.Application.Offers.Commands
{
    public class SendOfferEmailCommand : IRequest<bool>
    {
        public int OfferId { get; set; }
        
        public int UserId { get; set; }
    }
}
