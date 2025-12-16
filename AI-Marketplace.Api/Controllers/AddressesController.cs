using AI_Marketplace.Application.Addresses.Commands;
using AI_Marketplace.Application.Addresses.DTOs;
using AI_Marketplace.Application.Addresses.Queries;
using AI_Marketplace.Application.Common.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AI_Marketplace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles="Customer, Admin")]
    public class AddressesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AddressesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAddress(CreateAddressDto dto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null)
            {
                return Unauthorized();
            }

            if (!int.TryParse(userIdString, out var userId))
            {
                return BadRequest("Invalid User ID");
            }

            var command = new CreateAddressCommand(new CreateAddressRequestDto
            {
                City = dto.City,
                Country = dto.Country,
                IsPrimary = dto.IsPrimary,
                PostalCode = dto.PostalCode,
                State = dto.State,
                Street = dto.Street,
                UserId = userId
            });

            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAddress(UpdateAddressRequestDto dto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null)
                return Unauthorized();

            if (!int.TryParse(userIdString, out var userId))
                return BadRequest("Invalid User ID");

            var result = await _mediator.Send(
                new UpdateAddressCommand(userId, dto)
            );

            if (result == null)
                return NotFound("Address not found.");

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var deleted = await _mediator.Send(new DeleteAddressCommand(id));

            if (!deleted)
                return NotFound("Address not found.");

            return Ok(true);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAddressById(int id)
        {
            var result = await _mediator.Send(new GetAddressByIdQuery(id));

            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetAddressesForLoggedUser()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null)
            {
                return Unauthorized();
            }

            if (!int.TryParse(userIdString, out var userId))
            {
                return BadRequest("Invalid User ID");
            }
            var result = await _mediator.Send(new GetAddressesByUserIdQuery(userId));
            return Ok(ApiResponse<object>.Ok(result , "Addresses retrieved successfully"));
        }
    }
}
