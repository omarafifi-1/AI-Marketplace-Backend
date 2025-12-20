using AI_Marketplace.Application.Buyers.Queries.GetAllBuyerOrders;
using AI_Marketplace.Application.Common.DTOs;
using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Reviews.Commands;
using AI_Marketplace.Application.Reviews.DTOs;
using AI_Marketplace.Application.Reviews.Queries;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



namespace AI_Marketplace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IMediator _mediatR;
        private readonly IMapper _mapper;

        public ReviewsController(IMediator mediatR, IMapper mapper)
        {
            _mediatR = mediatR;
            _mapper = mapper;
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetAllReviewsByProductId(int productId)
        {
            if (productId < 1)
                return BadRequest("Invlaid product Id");

            var command = new GetAllProductReviewsQuery(productId);
            var result = await _mediatR.Send(command);
            return Ok(ApiResponse<List<ReviewDto>>.Ok(result , "All Products feteched successfully"));
        }


        [HttpGet("store/{storeId}")]
        public async Task<IActionResult> GetAllReviewsByStoreId(int storeId)
        {
            if (storeId < 1)
                return BadRequest("Invalid store Id");

            var query = new GetAllStoreReviewsQuery(storeId);
            var result = await _mediatR.Send(query);
            return Ok(ApiResponse<List<ReviewDto>>.Ok(result, "All Store reviews fetched successfully"));
        }

        [HttpPost("create/store")]
        public async Task<IActionResult> CreateStoreReview([FromBody] StoreReviewDto storeReviewDto)
        {
            if (storeReviewDto == null)
                return BadRequest("Review data is required");

            var command = new CreateStoreReviewCommand(storeReviewDto);
            var reviewId = await _mediatR.Send(command);
            return Ok(ApiResponse<int>.Ok(reviewId, "Store review created successfully"));
        }

        [Authorize]
        [HttpPost("create/product")]
        public async Task<IActionResult> CreateProductReview(
            [FromBody] ProductReviewDto productReviewDto,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = int.Parse(
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

            var command = new CreateProductReviewCommand(productReviewDto, userId);

            var reviewId = await _mediatR.Send(command, cancellationToken);

            return CreatedAtAction(
                nameof(CreateProductReview),
                ApiResponse<int>.Ok(reviewId, "Product review created successfully")
            );
        }



        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> DeleteReviewById(int reviewId)
        {
            if (reviewId < 1)
                return BadRequest("Invalid review Id");

            var command = new DeleteReviewByIdCommand(reviewId);
            var success = await _mediatR.Send(command);
            if (!success)
                return NotFound("Review not found");

            return Ok(ApiResponse<bool>.Ok(true, "Review deleted successfully"));
        }

    }
}
