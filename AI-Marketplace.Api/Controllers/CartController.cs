using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AI_Marketplace.Application.Carts.Queries;
using AI_Marketplace.Application.Carts.DTOs;
using AI_Marketplace.Application.Carts.Commands;
using AI_Marketplace.Application.Common.DTOs;
using System.Security.Claims;

namespace AI_Marketplace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CartController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize(Roles = "Customer, Admin")]
        [ProducesResponseType(typeof(ApiResponse<CartDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCartById()
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

            var query = new GetCartByUserIdQuery(userId);
            var result = await _mediator.Send(query);

            if (result is null)
                return NotFound("Cart not found.");

            return Ok(ApiResponse<CartDto>.Ok(result, "Cart retrieved successfully"));
        }

        [HttpPost("add")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<CartDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddProductToCart([FromBody] AddProductToCartDto dto)
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

            var command = new AddProdToCartCommand(userId, dto.ProductId);
            var result = await _mediator.Send(command);
            
            if (result is null)
                return NotFound("Product not found or inactive.");

            return Ok(ApiResponse<CartDto>.Ok(result, "Item added to cart"));
        }

        [HttpDelete("item-delete")]
        [Authorize(Roles = "Customer, Admin")]
        [ProducesResponseType(typeof(ApiResponse<CartDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoveSpecificCartItem([FromBody] RemoveProductFromCartDto dto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null)
                return Unauthorized();
            
            if (!int.TryParse(userIdString, out var userId))
                return BadRequest("Invalid User ID");

            var command = new RemoveProdFromCartCommand(userId, dto.ProductId);
            var result = await _mediator.Send(command);
            
            if (result is null)
                return NotFound("Cart or product not found in cart.");

            return Ok(ApiResponse<CartDto>.Ok(result, "Item removed from cart"));
        }

        [HttpPut("update-quantity")]
        [Authorize(Roles = "Customer, Admin")]
        [ProducesResponseType(typeof(ApiResponse<CartDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateCartItemQuantity([FromBody] UpdateCartQuantityDto updateDto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null)
                return Unauthorized();

            if (!int.TryParse(userIdString, out var userId))
                return BadRequest("Invalid User ID");

            var command = new UpdateCartQuantityCommand(userId, updateDto.ProductId, updateDto.Quantity);
            var result = await _mediator.Send(command);

            if (result is null)
                return NotFound("Cart or product not found in cart.");

            return Ok(ApiResponse<CartDto>.Ok(result, "Cart item updated"));
        }

        [HttpDelete("clear-cart")]
        [Authorize(Roles = "Customer, Admin")]
        [ProducesResponseType(typeof(ApiResponse<CartDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ClearUserCart()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null)
                return Unauthorized();
                
            if (!int.TryParse(userIdString, out var userId))
                return BadRequest("Invalid User ID");
                
            var command = new ClearUserCartCommand(userId);
            var result = await _mediator.Send(command);
            
            if (result is null)
                return NotFound("Cart not found.");
                
            return Ok(ApiResponse<CartDto>.Ok(result, "Cart cleared"));
        }
    }
}
