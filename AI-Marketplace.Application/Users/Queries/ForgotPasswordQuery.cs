using AI_Marketplace.Application.Users.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Users.Queries
{
    public record ForgotPasswordQuery(string Email) : IRequest<ForgotPasswordResponseDto>
    {
    }
}
