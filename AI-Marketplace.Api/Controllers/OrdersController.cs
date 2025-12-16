using AI_Marketplace.Application.Common.DTOs;
using AI_Marketplace.Application.Orders.Commands;
using AI_Marketplace.Application.Orders.DTOs;
using AI_Marketplace.Application.Orders.Queries;
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
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Buyer,Seller,Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            var query = new GetOrderByIdQuery(id, userId);
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound(ApiResponse<object>.Fail(new[] { "Order not found" }, "The requested order does not exist."));
            }

            return Ok(ApiResponse<object>.Ok(result, "Order retrieved successfully."));
        }

        [HttpPost("create")]
        [Authorize(Roles = "Customer, Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateOrderAsync([FromBody] CreateOrderDto dto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int.TryParse(userIdString, out var UserIdint);

            if (string.IsNullOrWhiteSpace(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            var command = new CreateOrdersFromCartCommand(UserIdint, null, dto.ShippingAddress);
            var results = await _mediator.Send(command);

            return Ok(ApiResponse<object>.Ok(results, "Orders created successfully."));
        }

        [HttpPost("from-cart/{cartId}")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateOrdersFromCartIdAsync(int cartId, [FromBody] string? shippingAddress)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            var command = new CreateOrdersFromCartCommand(userId, cartId, shippingAddress);
            var results = await _mediator.Send(command);

            return Ok(ApiResponse<object>.Ok(results, "Orders created successfully from cart."));
        }

        [HttpPost("{orderId}/items")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddOrderItemAsync(int orderId, [FromBody] AddOrderItemDto dto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            var command = new AddOrderItemCommand
            {
                OrderId = orderId,
                UserId = userId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            };

            var result = await _mediator.Send(command);

            return Ok(ApiResponse<object>.Ok(result, "Order item added successfully."));
        }

        [HttpPut("{orderId}/items/{itemId}/quantity")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateOrderItemQuantityAsync(int orderId, int itemId, [FromBody] UpdateOrderItemQuantityDto dto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            var command = new UpdateOrderItemQuantityCommand
            {
                OrderId = orderId,
                OrderItemId = itemId,
                UserId = userId,
                NewQuantity = dto.NewQuantity
            };

            var result = await _mediator.Send(command);

            return Ok(ApiResponse<object>.Ok(result, "Order item quantity updated successfully."));
        }

        [HttpDelete("{orderId}/items/{itemId}")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RemoveOrderItemAsync(int orderId, int itemId)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            var command = new RemoveOrderItemCommand(orderId, itemId, userId);
            var result = await _mediator.Send(command);

            return Ok(ApiResponse<object>.Ok(result, "Order item removed successfully."));
        }

        [HttpPost("{id}/cancel")]
        [Authorize(Roles = "Customer,Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CancelOrderAsync(int id)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            var command = new CancelOrderCommand(id, userId);
            var result = await _mediator.Send(command);

            return Ok(ApiResponse<object>.Ok(result, "Order cancelled successfully."));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteOrderAsync(int id)
        {
            var command = new DeleteOrderCommand(id);
            var result = await _mediator.Send(command);

            return Ok(ApiResponse<object>.Ok(result, "Order deleted successfully."));
        }

        [HttpGet("summary")]
        [Authorize(Roles = "Customer,Seller,Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetOrdersSummaryAsync()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var query = new GetOrdersSummaryQuery(userId, userRole);
            var result = await _mediator.Send(query);

            return Ok(ApiResponse<object>.Ok(result, "Orders summary retrieved successfully."));
        }

        [HttpPost("{id}/status")]
        [Authorize(Roles = "Seller,Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAllOrders()
        {
            var query = new GetAllOrdersQuery();
            var result = await _mediator.Send(query);

            return Ok(ApiResponse<object>.Ok(result, "Orders retrieved successfully."));
        }

        [HttpGet("user")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllOrdersForLoggedUser()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            var query = new GetAllOrdersForUserQuery(userId);
            var result = await _mediator.Send(query);

            return Ok(ApiResponse<object>.Ok(result, "User orders retrieved successfully."));
        }

        [HttpGet("store")]
        [Authorize(Roles = "Seller,Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetOrdersForStore()
        {
            var storeIdClaim = User.FindFirst("storeId")?.Value;
            if (string.IsNullOrWhiteSpace(storeIdClaim) || !int.TryParse(storeIdClaim, out var storeId))
            {
                return BadRequest(ApiResponse<object>.Fail(new[] { "Invalid Store ID" }, "Missing or invalid store identifier."));
            }
            
            var result = await _mediator.Send(new GetOrdersByStoreIdQuery(storeId));
            return Ok(ApiResponse<object>.Ok(result, "Store orders retrieved successfully."));
        }
    }
}
