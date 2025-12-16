using AI_Marketplace.Application.CustomRequests.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.CustomRequests.Queries
{
    public class GetCustomRequestByUserIdQuery : IRequest<List<CustomRequestResponseDto>>
    {
        public int UserId { get; set; }
    }
}
