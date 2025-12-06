using AI_Marketplace.Application.Admin.Commands;
using AI_Marketplace.Application.Admin.Queries;
using AI_Marketplace.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AI_Marketplace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetPlatformStats()
        {
            var stats = await _mediator.Send(new GetPlatformStatsQuery());
            return Ok(stats);
        }

        [HttpGet("vendors/pending")]
        public async Task<IActionResult> GetPendingVendors()
        {
            var pendingVendors = await _mediator.Send(new GetPendingVendorsQuery());
            return Ok(pendingVendors);
        }

        [HttpPut("vendors/{storeId}/approve")]
        public async Task<IActionResult> ApproveVendor(int storeId)
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
            var result = await _mediator.Send(new ApproveVendorCommand { StoreId = storeId , AdminId = UserIdInt });
            return Ok(result);
        }

        [HttpPut("vendors/{storeId}/reject")]
        public async Task<IActionResult> RejectVendor(int storeId)
        {
            var result = await _mediator.Send(new RejectVendorCommand { StoreId = storeId });
            return Ok(result);
        }
    }
}
