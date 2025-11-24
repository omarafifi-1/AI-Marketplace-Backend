using AI_Marketplace.Application.Offers.Commands;
using AI_Marketplace.Application.Offers.DTOs;
using AI_Marketplace.Application.Offers.Queries.GetOffersByCustomRequestId;
using AI_Marketplace.Application.Offers.Queries.GetOffersByStoreId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AI_Marketplace.Controllers
{
    /// <summary>
    /// Controller for managing vendor offers on custom requests.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class OffersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OffersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Creates a new offer for a custom request. Only vendors with active stores can create offers.
        /// </summary>
        /// <param name="dto">The offer details</param>
        /// <returns>The created offer</returns>
        /// <response code="201">Offer created successfully</response>
        /// <response code="400">Validation errors (invalid price, days, or message length)</response>
        /// <response code="401">User is not authenticated</response>
        /// <response code="403">User is not a vendor or has no active store</response>
        /// <response code="409">Store has already submitted an offer for this custom request</response>
        [HttpPost]
        [Authorize(Roles = "Vendor,Seller")]
        [ProducesResponseType(typeof(OfferResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<OfferResponseDto>> CreateOffer([FromBody] CreateOfferDto dto)
        {
            // Extract user ID from JWT claims
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Invalid user authentication." });
            }

            // Create command with user ID and DTO data
            var command = new CreateOfferCommand
            {
                UserId = userId,
                CustomRequestId = dto.CustomRequestId,
                ProposedPrice = dto.ProposedPrice,
                EstimatedDays = dto.EstimatedDays,
                Message = dto.Message
            };

            var result = await _mediator.Send(command);

            // Return 201 Created with Location header
            return CreatedAtAction(
                nameof(GetOffersByCustomRequest),
                new { customRequestId = result.CustomRequestId },
                result);
        }

        /// <summary>
        /// Retrieves all offers for a specific custom request. Any authenticated user can view offers.
        /// </summary>
        /// <param name="customRequestId">The ID of the custom request</param>
        /// <returns>List of offers for the custom request</returns>
        /// <response code="200">Returns list of offers (empty array if none exist)</response>
        /// <response code="401">User is not authenticated</response>
        /// <response code="404">Custom request not found</response>
        [HttpGet("customrequest/{customRequestId:int}")]
        [Authorize]
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

        /// <summary>
        /// Retrieves paginated offers for the authenticated vendor's store.
        /// </summary>
        /// <param name="page">Page number (1-based, default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 20, max: 100)</param>
        /// <returns>Paginated list of vendor's offers</returns>
        /// <response code="200">Returns paginated offers for the vendor's store</response>
        /// <response code="400">Vendor has no active store</response>
        /// <response code="401">User is not authenticated</response>
        /// <response code="403">User is not a vendor</response>
        [HttpGet("mystore")]
        [Authorize(Roles = "Vendor,Seller")]
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
    }
}
