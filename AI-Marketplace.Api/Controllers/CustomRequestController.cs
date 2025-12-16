using AI_Marketplace.Application.CustomRequests.Commands;
using AI_Marketplace.Application.CustomRequests.DTOs;
using AI_Marketplace.Application.CustomRequests.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AI_Marketplace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomRequestController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CustomRequestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("custom-request")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(CustomRequestResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateCustomRequest([FromBody] CreateCustomRequestDto dto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null)
            {
                return Unauthorized();
            }
            
            if (!int.TryParse(userIdString, out int userIdInt))
            {
                return BadRequest("Invalid User ID");
            }
            var command = new CreateCustomRequestCommand
            {
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                UserId = userIdInt,
                ImageUrl = dto.ImageUrl,
                Budget = dto.Budget,
                Deadline = dto.Deadline
            };
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetCustomRequestById),new { id = result.Id },result);
        }


        [HttpGet("{id:int}")]
        [Authorize(Roles = "Customer,Seller,Admin")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCustomRequestById(int id)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
             return Unauthorized(new { message = "Invalid user authentication." });
            
            // Extract UserRole
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(userRole))
                return Unauthorized(new { message = "User role not found." });
            

            var query = new GetCustomRequestByIdQuery
            {
                Id = id,
                UserId = userId,      
                UserRole = userRole
            };
            var result = await _mediator.Send(query);
            if (result is null) return NotFound();
            return Ok(result);
        }


        [HttpPut("{id:int}")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCustomRequest(int id, [FromBody] UpdateCustomRequestDto updateCustomRequestDto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null)
            {
                return Unauthorized();
            }
            
            if (!int.TryParse(userIdString, out int userIdInt))
            {
                return BadRequest("Invalid User ID");
            }

            var command = new UpdateCustomRequestCommand
            {
                Id = id,
                Description = updateCustomRequestDto.Description,
                CategoryId = updateCustomRequestDto.CategoryId,
                UserId = userIdInt,
                ImageUrl = updateCustomRequestDto.ImageUrl,
                Budget = updateCustomRequestDto.Budget,
                Deadline = updateCustomRequestDto.Deadline

            };
            var result = await _mediator.Send(command);
            if (result is null) return NotFound();
            return Ok(result);
        }


        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Customer,Admin")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCustomRequest(int id)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null)
            {
                return Unauthorized();
            }
            
            if (!int.TryParse(userIdString, out int userIdInt))
            {
                return BadRequest("Invalid User ID");
            }

            var command = new DeleteCustomRequestCommand
            {
                Id = id,
                UserId = userIdInt
            };
            var result = await _mediator.Send(command);
            if (!result) return NotFound();
            return NoContent();

        }


        [HttpGet]
        [Authorize(Roles = "Seller,Admin")]
        [ProducesResponseType(typeof(List<CustomRequestResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllCustomRequest([FromQuery] string? status = null,
            [FromQuery] int? categoryId = null)
        {
            var query = new GetAllCustomRequestQuery
            {
                Status = status,
                CategoryId = categoryId
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }


        [HttpGet("my-requests")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(List<CustomRequestResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMyCustomRequests()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { message = "Invalid user authentication." });
            }

            var query = new GetCustomRequestByUserIdQuery
            {
                UserId = userId
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }



    }
}
