using AI_Marketplace.Application.Users.DTOs;
using MediatR;

namespace AI_Marketplace.Application.Users.Commands.RegisterUser
{
    public class RegisterUserCommand : IRequest<UserResponseDto>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string? Role { get; set; }
    }
}