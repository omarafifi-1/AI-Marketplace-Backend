using AI_Marketplace.Application.CustomRequests.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.CustomRequests.Queries
{
    public class GetAllCustomRequestQuery : IRequest<List<CustomRequestResponseDto>>
    {
        // optional filtere if needed in future 
        public string? Status { get; set; }
        public int? CategoryId { get; set; }
    }
}
