using AI_Marketplace.Application.Users.Commands.RegisterUser;
using AI_Marketplace.Application.Users.DTOs;
using AI_Marketplace.Application.Users.Queries.GetAllUsers;
using MediatR;
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
        [ProducesResponseType(typeof(UserResponseDto), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<UserResponseDto>> Register([FromBody] RegisterUserDto dto)
        {
            var command = new RegisterUserCommand
            {
                Email = dto.Email,
                Password = dto.Password,
                UserName = dto.UserName,
                Role = dto.Role
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }


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