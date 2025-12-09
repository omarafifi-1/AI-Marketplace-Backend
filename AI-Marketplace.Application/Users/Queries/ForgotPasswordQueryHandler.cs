using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Users.DTOs;
using AI_Marketplace.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AI_Marketplace.Application.Users.Queries
{
    public class ForgotPasswordQueryHandler : IRequestHandler<ForgotPasswordQuery, DTOs.ForgotPasswordResponseDto>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailSender;

        public ForgotPasswordQueryHandler(
            UserManager<ApplicationUser> userManager,
            IEmailService emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }
        public async Task<ForgotPasswordResponseDto> Handle(ForgotPasswordQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                return new ForgotPasswordResponseDto
                {
                    Message = "No user found with this email."
                };
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);
            var encodedEmail = WebUtility.UrlEncode(user.Email);

            // This will be handled by Angular route
            var resetUrl = $"http://localhost:4200/auth/reset-password?token={encodedToken}&email={encodedEmail}";

            var subject = "Reset Your Password";
            var body = $@"
                <p>Click the link below to reset your password:</p>
                <a href=""{resetUrl}"">Reset Password</a>
            ";

            await _emailSender.SendEmailAsync(user.Email!, subject, body);

            return new ForgotPasswordResponseDto
            {
                Message = "Reset password email has been sent."
            };
        }
    }
}
