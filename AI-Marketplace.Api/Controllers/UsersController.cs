using AI_Marketplace.Application.Users.Commands;
using AI_Marketplace.Application.Users.Commands.UpdateUserProfile;
using AI_Marketplace.Application.Users.DTOs;
using AI_Marketplace.Application.Users.Queries;
using AI_Marketplace.Application.Users.Queries.GetAllUsers;
using AI_Marketplace.Application.Users.Queries.GetUserProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
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

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var UserIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserIdString == null)
            {
                return Unauthorized();
            }
            var UserIdInt = int.Parse(UserIdString);
            if (!int.TryParse(UserIdString, out UserIdInt))
            {
                return BadRequest("Invalid User ID");
            }

            var query = new GetUserProfileQuery(UserIdInt);

            var dto = await _mediator.Send(query);
            return Ok(dto);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileDto dto)
        {
            var UserIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserIdString == null)
            {
                return Unauthorized();
            }
            var UserIdInt = int.Parse(UserIdString);
            if (!int.TryParse(UserIdString, out UserIdInt))
            {
                return BadRequest("Invalid User ID");
            }

            var query = new UpdateUserProfileCommand(UserIdInt, dto);

            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto dto)
        {
            var result = await _mediator.Send(new ForgotPasswordQuery(dto.Email));
            return Ok(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto dto)
        {
            var command = new ResetPasswordCommand(
                dto.Email,
                dto.Token,
                dto.NewPassword,
                dto.ConfirmPassword
            );

            var result = await _mediator.Send(command);

            if (!result.Succeeded)
                return BadRequest(result);  

            return Ok(result);
        }

    }
}