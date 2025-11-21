using AI_Marketplace.Application.Users.Commands;
using AI_Marketplace.Application.Users.DTOs;
using AI_Marketplace.Application.Users.Queries.GetAllUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AI_Marketplace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserResponseDto>> Register([FromBody] RegisterUserDto dto)
        {
            var command = new RegisterUserCommand
            {
                Email = dto.Email,
                Password = dto.Password,
                RePassword = dto.RePassword,
                UserName = dto.UserName,
                Role = dto.Role
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto dto)
        {   
            var command = new LoginUserCommand
            {
                Email = dto.Email,
                Password = dto.Password
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ProducesResponseType(typeof(List<UserResponseDto>), 200)]
        public async Task<ActionResult<List<UserResponseDto>>> GetAllUsers()
        {
            var query = new GetAllUsersQuery();
            var users = await _mediator.Send(query);
            return Ok(users);
        }
    }
}