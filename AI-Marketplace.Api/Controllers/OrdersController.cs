using AI_Marketplace.Application.Orders.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AI_Marketplace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator) 
        {
            _mediator  = mediator;
        }

        [HttpPost("{id}/status")]
        [Authorize(Roles = "Seller,Admin")]
        public async Task<IActionResult> ChangeOrderStatus(int id,[FromBody] string status)
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
            var command = new ChangeOrderStatusCommand(id, status, UserIdInt);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
