using AI_Marketplace.Application.Offers.Commands;
using AI_Marketplace.Application.Offers.DTOs;
using AI_Marketplace.Application.Offers.Queries;
using AI_Marketplace.Application.Offers.Queries.GetOffersByCustomRequestId;
using AI_Marketplace.Application.Offers.Queries.GetOffersByStoreId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AI_Marketplace.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class OffersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OffersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize(Roles = "Vendor,Seller")]
        [ProducesResponseType(typeof(OfferResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<OfferResponseDto>> CreateOffer([FromBody] CreateOfferDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Invalid user authentication." });
            }

            var command = new CreateOfferCommand
            {
                UserId = userId,
                CustomRequestId = dto.CustomRequestId,
                ProposedPrice = dto.ProposedPrice,
                EstimatedDays = dto.EstimatedDays,
                Message = dto.Message
            };

            var result = await _mediator.Send(command);

            return CreatedAtAction(
                nameof(GetOffersByCustomRequest),
                new { customRequestId = result.CustomRequestId },
                result);
        }

        [HttpGet("customrequest/{customRequestId:int}")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(List<OfferResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<OfferResponseDto>>> GetOffersByCustomRequest(int customRequestId)
        {
            var query = new GetOffersByCustomRequestIdQuery
            {
                CustomRequestId = customRequestId
            };

            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpGet("mystore")]
        [Authorize(Roles = "Seller")]
        [ProducesResponseType(typeof(PagedOfferDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PagedOfferDto>> GetMyStoreOffers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            // Extract user ID from JWT claims
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Invalid user authentication." });
            }

            var query = new GetOffersByStoreIdQuery
            {
                UserId = userId,
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpPut("{id:int}/accept")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderResponseDto>> AcceptOffer(int id, [FromBody] AcceptOfferDto dto)
        {
            // Extract user ID from JWT claims
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Invalid user authentication." });
            }

            var command = new AcceptOfferCommand
            {
                OfferId = id,
                UserId = userId,
                ShippingAddress = dto.ShippingAddress
            };

            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Vendor,Seller")]
        [ProducesResponseType(typeof(OfferResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OfferResponseDto>> UpdateOffer(int id, [FromBody] UpdateOfferDto dto)
        {
            // Extract user ID from JWT claims
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Invalid user authentication." });
            }

            var command = new UpdateOfferCommand
            {
                OfferId = id,
                UserId = userId,
                ProposedPrice = dto.ProposedPrice,
                EstimatedDays = dto.EstimatedDays,
                Message = dto.Message
            };

            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        [ProducesResponseType(typeof(OfferResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OfferResponseDto>> GetOfferById(int id)
        {
            var query = new GetOfferByIdQuery
            {
                OfferId = id
            };

            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound(new { message = $"Offer with ID {id} not found." });
            }

            return Ok(result);
        }
    }
}
