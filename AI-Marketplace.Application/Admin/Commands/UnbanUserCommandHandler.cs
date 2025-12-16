using AI_Marketplace.Application.Common.Exceptions;
using AI_Marketplace.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Admin.Commands
{
    public class UnbanUserCommandHandler : IRequestHandler<UnbanUserCommand, string>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UnbanUserCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<string> Handle(UnbanUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                throw new NotFoundException(new Dictionary<string, string[]>
                {
                    { "User", new[] { "User Not Found." } }
                });
            }
            user.IsActive = true;
            await _userManager.UpdateAsync(user);
            return "User Has Been Unbanned Successfully.";
        }
    }
}
