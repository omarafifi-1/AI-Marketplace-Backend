using AI_Marketplace.Application.Users.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Users.Commands
{
    public class RefreshTokenCommand : IRequest<LoginResponseDto>
    {
        public string RefreshToken { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
    }
}
