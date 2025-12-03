using AI_Marketplace.Application.Common.Exceptions;
using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Users.DTOs;
using AI_Marketplace.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AI_Marketplace.Application.Users.Commands
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, UserResponseDto>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStoreRepository _storerepo;
        private readonly ICartRepository _cartRepo;

        public RegisterUserCommandHandler(UserManager<ApplicationUser> userManager, IStoreRepository storerepo, ICartRepository cartRepo)
        {
            _userManager = userManager;
            _storerepo = storerepo;
            _cartRepo = cartRepo;
        }

        public async Task<UserResponseDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // Check if passwords match
            if (request.Password != request.RePassword)
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Password", new[] { "Passwords Do Not Match." } }
                });
            }

            // Validate role
            var validRoles = new[] { "Admin", "Seller", "Customer" };
            var role = string.IsNullOrWhiteSpace(request.Role) ? "Customer" : request.Role;
            
            if (!validRoles.Contains(role))
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Role", new[] { $"Invalid Role '{role}'. Valid Roles are: Admin, Seller, Customer" } }
                });
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
                var errors = result.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Description).ToArray()
                    );

                throw new ValidationException(errors);
            }

            // Assign role using UserManager after user creation
            var roleResult = await _userManager.AddToRoleAsync(user, role);
            
            if (!roleResult.Succeeded)
            {
                var errors = roleResult.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Description).ToArray()
                    );

                throw new ValidationException(errors);
            }

            // Create an empty cart for the new user
            var userCart = new Cart
            {
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _cartRepo.CreateNewCartAsync(userCart, cancellationToken);

            // Create a store for the seller
            Store? store = null;
            if (role == "Seller")
            {
                var st = new Store
                {
                    StoreName = $"{user.UserName}'s Store",
                    OwnerId = user.Id,
                    ContactEmail = user.Email,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    IsVerified = false
                };
                store = await _storerepo.CreateAsync(st, cancellationToken);
            }

            // Get the actual assigned roles
            var userRoles = await _userManager.GetRolesAsync(user);

            return new UserResponseDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = userRoles.FirstOrDefault() ?? string.Empty,
                CreatedAt = user.CreatedAt,
                StoreId = store?.Id,
                StoreName = store?.StoreName
            };
        }
    }
}