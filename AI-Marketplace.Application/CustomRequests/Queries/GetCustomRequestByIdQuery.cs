using AI_Marketplace.Application.CustomRequests.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.CustomRequests.Queries
{
    public class GetCustomRequestByIdQuery : IRequest<CustomRequestResponseDto>
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserRole { get; set; } = default!;
    }
}
