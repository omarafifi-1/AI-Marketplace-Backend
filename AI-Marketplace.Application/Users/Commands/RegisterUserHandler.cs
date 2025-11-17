using AI_Marketplace.Application.Users.DTOs;
using AI_Marketplace.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AI_Marketplace.Application.Users.Commands.RegisterUser
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, UserResponseDto>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public RegisterUserCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserResponseDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // Validate role
            var validRoles = new[] { "Admin", "Seller", "Customer" };
            var role = string.IsNullOrWhiteSpace(request.Role) ? "Customer" : request.Role;
            
            if (!validRoles.Contains(role))
            {
                throw new Exception($"Invalid role '{role}'. Valid roles are: Admin, Seller, Customer");
            }

            var user = new ApplicationUser
            {
                UserName = request.UserName,
                Email = request.Email,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"User registration failed: {errors}");
            }

            // Assign role using UserManager after user creation
            var roleResult = await _userManager.AddToRoleAsync(user, role);
            
            if (!roleResult.Succeeded)
            {
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                throw new Exception($"Role assignment failed: {errors}");
            }

            // Get the actual assigned roles
            var userRoles = await _userManager.GetRolesAsync(user);

            return new UserResponseDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = userRoles.FirstOrDefault() ?? string.Empty,
                CreatedAt = user.CreatedAt
            };
        }
    }
}