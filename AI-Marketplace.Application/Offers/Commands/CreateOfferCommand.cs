using System;
using System.Collections.Generic;
using System.Text;
using AI_Marketplace.Application.Offers.DTOs;
using MediatR;

namespace AI_Marketplace.Application.Offers.Commands
{
    public class CreateOfferCommand : IRequest<OfferResponseDto>
    {
        public int UserId { get; set; }

        public int CustomRequestId { get; set; }

        public decimal ProposedPrice { get; set; }

        public int EstimatedDays { get; set; }

        public string? Message { get; set; }
    }
}
