using AI_Marketplace.Application.Users.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Users.Commands
{
    public record ResetPasswordCommand(
      string Email,
      string Token,
      string NewPassword,
      string ConfirmPassword
  ) : IRequest<ResetPasswordResponseDto>;
}
