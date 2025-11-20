using AI_Marketplace.Application.Common.Exceptions;
using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Users.DTOs;
using AI_Marketplace.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AI_Marketplace.Application.Users.Commands
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginResponseDto>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IStoreRepository _storeRepository;

        public LoginUserCommandHandler(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtTokenService jwtTokenService,
            IStoreRepository storeRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenService = jwtTokenService;
            _storeRepository = storeRepository;
        }

        public async Task<LoginResponseDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            // Find user by email
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Email", new[] { "Invalid email or password." } }
                });
            }

            // Check if user is active
            if (!user.IsActive)
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Account", new[] { "Your account has been deactivated." } }
                });
            }

            // Verify password
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    throw new ValidationException(new Dictionary<string, string[]>
                    {
                        { "Account", new[] { "Account locked out due to multiple failed login attempts." } }
                    });
                }

                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Email", new[] { "Invalid email or password." } }
                });
            }

            // Get user roles
            var roles = await _userManager.GetRolesAsync(user);

            // Load store if user is a seller
            Store? store = null;
            if (roles.Contains("Seller"))
            {
                store = await _storeRepository.GetByOwnerIdAsync(user.Id, cancellationToken);
            }

            // Generate JWT token
            var token = _jwtTokenService.GenerateToken(user, roles);

            // Return response
            return new LoginResponseDto
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddDays(30),
                User = new UserResponseDto
                {
                    Id = user.Id,
                    UserName = user.UserName!,
                    Email = user.Email!,
                    Role = roles.FirstOrDefault() ?? string.Empty,
                    CreatedAt = user.CreatedAt,
                    StoreId = store?.Id,
                    StoreName = store?.StoreName
                }
            };
        }
    }
}
