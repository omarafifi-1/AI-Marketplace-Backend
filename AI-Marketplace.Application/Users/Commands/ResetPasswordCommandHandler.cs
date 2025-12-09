using AI_Marketplace.Application.Users.DTOs;
using AI_Marketplace.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AI_Marketplace.Application.Users.Commands
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ResetPasswordResponseDto>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ResetPasswordCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<ResetPasswordResponseDto> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            if (request.NewPassword != request.ConfirmPassword)
            {
                return new ResetPasswordResponseDto
                {
                    Succeeded = false,
                    Message = "Passwords do not match."
                };
            }

            var email = WebUtility.UrlDecode(request.Email);
            var token = request.Token.Replace(" ", "+");

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return new ResetPasswordResponseDto
                {
                    Succeeded = false,
                    Message = "Invalid email or token."
                };
            }

            var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);

            if (!result.Succeeded)
            {
                return new ResetPasswordResponseDto
                {
                    Succeeded = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }

            return new ResetPasswordResponseDto
            {
                Succeeded = true,
                Message = "Password reset successfully."
            };
        }
    }
}
