using AI_Marketplace.Application.Common.DTOs;
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
            SetRefreshTokenCookie(result.RefreshToken!, result.RefreshTokenExpiration);
            result.RefreshToken = null;

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(ApiResponse<LoginResponseDto>.Fail(
                    new[] { "Refresh token not found" },
                    "Unauthorized"
                ));
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var command = new RefreshTokenCommand
            {
                RefreshToken = refreshToken,
                IpAddress = ipAddress
            };
            
            var result = await _mediator.Send(command);

            SetRefreshTokenCookie(result.RefreshToken!, result.RefreshTokenExpiration);

            result.RefreshToken = null;

            return Ok(ApiResponse<LoginResponseDto>.Ok(result, "Token refreshed successfully"));
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("refreshToken");
            return Ok(new { message = "Logged Out Successfully" });
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
            if (result.Message == "No user found with this email.")
            {
                return BadRequest(new { message = result.Message });
            }
            return Ok(new { message = result.Message });
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

        // Helper method to set HttpOnly cookie
        private void SetRefreshTokenCookie(string refreshToken, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,   
                Expires = expires,
                Path = "/"                    
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}