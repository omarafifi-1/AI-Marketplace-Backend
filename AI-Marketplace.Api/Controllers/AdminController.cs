using AI_Marketplace.Application.Admin.Commands;
using AI_Marketplace.Application.Admin.Queries;
using AI_Marketplace.Application.Users.DTOs;
using AI_Marketplace.Application.Users.Queries.GetAllUsers;
using AI_Marketplace.Application.Vendors.Queries;
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
            var result = await _mediator.Send(new ApproveVendorCommand { StoreId = storeId, AdminId = UserIdInt });
            return Ok(result);
        }

        [HttpPut("vendors/{storeId}/reject")]
        public async Task<IActionResult> RejectVendor(int storeId)
        {
            var result = await _mediator.Send(new RejectVendorCommand { StoreId = storeId });
            return Ok(result);
        }

        [HttpGet("vendors/all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllVendors()
        {
            var query = new GetAllVendorsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("Analytics")]
        public async Task<IActionResult> GetAnalyticsData()
        {
            var analyticsData = await _mediator.Send(new GetAnalyticsDataQuery());
            return Ok(analyticsData);
        }

        [HttpPut("users/{userId}/ban")]
        public async Task<IActionResult> BanUser(int userId)
        {
            var result = await _mediator.Send(new BanUserCommand { UserId = userId });
            return Ok(result);
        }

        [HttpPut("users/{userId}/unban")]
        public async Task<IActionResult> UnbanUser(int userId)
        {
            var result = await _mediator.Send(new UnbanUserCommand { UserId = userId });
            return Ok(result);
        }

        [HttpGet("Users")]
        [ProducesResponseType(typeof(List<UserResponseDto>), 200)]
        public async Task<ActionResult<List<UserResponseDto>>> GetAllUsers()
        {
            var query = new GetAllUsersQuery();
            var users = await _mediator.Send(query);
            return Ok(users);
        }
    }
}
