using AI_Marketplace.Application.Addresses.Commands;
using AI_Marketplace.Application.Addresses.DTOs;
using AI_Marketplace.Application.Addresses.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AI_Marketplace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AddressesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAddress(CreateAddressRequestDto dto)
        {
            var result = await _mediator.Send(new CreateAddressCommand(dto));
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAddress(UpdateAddressRequestDto dto)
        {
            var result = await _mediator.Send(new UpdateAddressCommand(dto));

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

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetAddressesByUserId(int userId)
        {
            var result = await _mediator.Send(new GetAddressesByUserIdQuery(userId));
            return Ok(result);
        }
    }
}
