using AI_Marketplace.Application.Common.Exceptions;
using AI_Marketplace.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Admin.Commands
{
    public class BanUserCommandHandler : IRequestHandler<BanUserCommand, string>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public BanUserCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<string> Handle(BanUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                throw new NotFoundException (new Dictionary<string, string[]>
                {
                    { "User", new[] { "User Not Found." } }
                });
            }
            user.IsActive = false;
            await _userManager.UpdateAsync(user);
            return "User Has Been Banned Successfully.";
        }
    }
}
