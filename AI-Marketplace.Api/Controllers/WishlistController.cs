using AI_Marketplace.Application.Common.DTOs;
using AI_Marketplace.Application.Wishlists.Commands;
using AI_Marketplace.Application.Wishlists.DTOs;
using AI_Marketplace.Application.Wishlists.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AI_Marketplace.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WishlistController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// Get the current user's wishlist
        [HttpGet]
        [Authorize(Roles = "Customer, Admin")]
        [ProducesResponseType(typeof(ApiResponse<WishlistDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetWishlist()
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

            var query = new GetWishlistByUserIdQuery(userId);
            var result = await _mediator.Send(query);

            return Ok(ApiResponse<WishlistDto>.Ok(result, "Wishlist retrieved successfully"));
        }

        /// Add a product to the user's wishlist
        [HttpPost("add")]
        [Authorize(Roles = "Customer, Admin")]
        [ProducesResponseType(typeof(ApiResponse<WishlistItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddToWishlist([FromBody] AddToWishlistDto dto)
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

            var command = new AddProductToWishlistCommand(userId, dto.ProductId);
            var result = await _mediator.Send(command);

            if (result == null)
            {
                return NotFound("Product not found or inactive");
            }

            return Ok(ApiResponse<WishlistItemDto>.Ok(result, "Product added to wishlist"));
        }

        /// Remove a product from the user's wishlist
        [HttpDelete("remove")]
        [Authorize(Roles = "Customer, Admin")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveFromWishlist([FromBody] RemoveFromWishlistDto dto)
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

            var command = new RemoveProductFromWishlistCommand(userId, dto.ProductId);
            var result = await _mediator.Send(command);

            if (!result)
            {
                return NotFound("Product not found in wishlist");
            }

            return Ok(ApiResponse<bool>.Ok(result, "Product removed from wishlist"));
        }

        /// Check if a specific product is in the user's wishlist
        [HttpGet("check/{productId}")]
        [Authorize(Roles = "Customer, Admin")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckProductInWishlist(int productId)
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

            var query = new CheckProductInWishlistQuery(userId, productId);
            var result = await _mediator.Send(query);

            return Ok(ApiResponse<bool>.Ok(result, result ? "Product is in wishlist" : "Product is not in wishlist"));
        }
    }
}
