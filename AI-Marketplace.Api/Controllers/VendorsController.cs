using AI_Marketplace.Application.Vendors.Commands;
using AI_Marketplace.Application.Vendors.DTOs;
using AI_Marketplace.Application.Vendors.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AI_Marketplace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Seller")]
    public class VendorsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public VendorsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("Profile")]
        public async Task<IActionResult> GetVendorProfile()
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
            var query = new GetVendorProfileQuery(UserIdInt);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("EditProfile")]
        public async Task<IActionResult> EditVendorProfile([FromBody] VendorEditDto vendorEditDto)
        {
            var UserIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserIdString == null)
            {
                return Unauthorized();
            }
            if (!int.TryParse(UserIdString, out int UserIdInt))
            {
                return BadRequest("Invalid User ID");
            }
            var command = new EditVendorProfileCommand(UserIdInt, vendorEditDto);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("Orders")]
        public async Task<IActionResult> GetVendorOrders()
        {
            var UserIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserIdString == null)
            {
                return Unauthorized();
            }
            if (!int.TryParse(UserIdString, out int UserIdInt))
            {
                return BadRequest("Invalid User ID");
            }
            var query = new GetVendorOrdersQuery(UserIdInt);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
