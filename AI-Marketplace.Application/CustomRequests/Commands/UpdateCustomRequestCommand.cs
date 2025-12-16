using AI_Marketplace.Application.CustomRequests.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.CustomRequests.Commands
{
    public class UpdateCustomRequestCommand : IRequest<CustomRequestResponseDto>
    {
        public int Id { get; set; }  
        public int UserId { get; set; }  
        public string? Description { get; set; }  
        public int? CategoryId { get; set; }

        public string? ImageUrl { get; set; }
        public decimal? Budget { get; set; }
        public DateTime? Deadline { get; set; }

    }
}
