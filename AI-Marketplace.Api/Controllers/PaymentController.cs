using AI_Marketplace.Application.Common.DTOs;
using AI_Marketplace.Application.Payment.Commands;
using AI_Marketplace.Application.Payment.DTO;
using AI_Marketplace.Application.Payment.DTOs;
using AI_Marketplace.Application.Payment.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace AI_Marketplace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IMediator mediator, ILogger<PaymentController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("create-intent")]
        [Authorize(Roles = "Customer,Admin")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreatePaymentIntentForUser([FromBody] PaymentDto paymentDto)
        {
            if (paymentDto is null)
            {
                return BadRequest(ApiResponse<string>.Fail(new[] { "Request body is required" }, "Invalid request"));
            }

            if (paymentDto.OrderId <= 0)
            {
                return BadRequest(ApiResponse<string>.Fail(new[] { "OrderId must be a positive integer" }, "Invalid order id"));
            }

            if (paymentDto.Amount <= 0)
            {
                return BadRequest(ApiResponse<string>.Fail(new[] { "Amount must be greater than 0" }, "Invalid amount"));
            }

            if (string.IsNullOrWhiteSpace(paymentDto.CurrencyCode))
            {
                return BadRequest(ApiResponse<string>.Fail(new[] { "CurrencyCode is required" }, "Invalid currency"));
            }

            // Ensure we have an authenticated user and pass it to the command
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(ApiResponse<string>.Fail(new[] { "Unauthorized" }, "User not authenticated"));
            }

            try
            {
                var command = new CreatePaymentIntentCommand
                {
                    OrderId = paymentDto.OrderId,
                    Amount = paymentDto.Amount,
                    Currency = paymentDto.CurrencyCode,

                };

                var clientSecret = await _mediator.Send(command);

                if (string.IsNullOrWhiteSpace(clientSecret))
                {
                    return BadRequest(ApiResponse<string>.Fail(new[] { "Failed to create payment intent" }, "Payment intent creation failed"));
                }

                return Ok(ApiResponse<string>.Ok(clientSecret, "Payment intent created successfully. Use this client secret with Stripe.js"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized to create payment intent for OrderId {OrderId}", paymentDto.OrderId);
                return Unauthorized(ApiResponse<string>.Fail(new[] { ex.Message }, "User not authorized"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogInformation(ex, "Business rule prevented creating payment intent for OrderId {OrderId}", paymentDto.OrderId);
                return BadRequest(ApiResponse<string>.Fail(new[] { ex.Message }, "Payment intent creation failed"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating payment intent for OrderId {OrderId}", paymentDto.OrderId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<string>.Fail(new[] { "An internal server error occurred" }, "Server error"));
            }               
        }




        [HttpGet("{id}")]
        [Authorize(Roles = "Customer,Seller,Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                return Unauthorized(ApiResponse<object>.Fail(new[] { "Unauthorized" }, "User not authenticated"));
            }

            var query = new GetPaymentByIdQuery(id, userId);
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound(ApiResponse<object>.Fail(new[] { "Payment not found" }, "The requested payment does not exist"));
            }

            return Ok(ApiResponse<object>.Ok(result, "Payment retrieved successfully"));
        }

        [HttpGet("order/{orderId}")]
        [Authorize(Roles = "Customer,Seller,Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetPaymentsByOrderId(int orderId)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                return Unauthorized(ApiResponse<object>.Fail(new[] { "Unauthorized" }, "User not authenticated"));
            }

            var query = new GetPaymentsByOrderIdQuery(orderId, userId);
            var result = await _mediator.Send(query);

            return Ok(ApiResponse<object>.Ok(result, "Payments retrieved successfully"));
        }

        [HttpPost("refund")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RefundPayment(RefundPaymentDto refundPaymentDto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                return Unauthorized(ApiResponse<object>.Fail(new[] { "Unauthorized" }, "User not authenticated"));
            }

            if (refundPaymentDto.RefundAmount <= 0)
            {
                return BadRequest(ApiResponse<object>.Fail(new[] { "Refund amount must be greater than 0" }, "Invalid refund amount"));
            }

            var command = new RefundPaymentCommand(refundPaymentDto.PaymentId, refundPaymentDto.RefundAmount, refundPaymentDto.RefundReason, userId);
            
            try
            {
                var result = await _mediator.Send(command);
                return Ok(ApiResponse<object>.Ok(result, "Refund processed successfully"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(new[] { ex.Message }, "Refund failed"));
            }
        }
    }
}
