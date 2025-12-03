using AI_Marketplace.Application.Common.DTOs;
using AI_Marketplace.Application.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace AI_Marketplace.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

            var response = context.Response;
            response.ContentType = "application/json";

            ApiResponse<object> payload;

            if (exception is ValidationException validationEx)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                payload = ApiResponse<object>.Fail(validationEx.Errors.SelectMany(kv => kv.Value), validationEx.Message);
            }
            else if (exception is NotFoundException notFoundEx)
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                payload = ApiResponse<object>.Fail(notFoundEx.Errors.SelectMany(kv => kv.Value), notFoundEx.Message);
            }
            else if (exception is DuplicateOfferException duplicateEx)
            {
                response.StatusCode = (int)HttpStatusCode.Conflict;
                payload = ApiResponse<object>.Fail(
                    Array.Empty<string>(),
                    duplicateEx.Message
                );
            }
            else if (exception is DbUpdateException dbEx &&
                     (dbEx.InnerException?.Message.Contains("IX_Offers_CustomRequestId_StoreId_Unique") == true ||
                      dbEx.InnerException?.Message.Contains("unique constraint", StringComparison.OrdinalIgnoreCase) == true))
            {
                _logger.LogWarning(dbEx, "Database unique constraint violation on Offers table");
                response.StatusCode = (int)HttpStatusCode.Conflict;
                payload = ApiResponse<object>.Fail(
                    Array.Empty<string>(),
                    "Store has already submitted an offer for this custom request."
                );
            }
            else if (exception is UnauthorizedAccessException)
            {
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                payload = ApiResponse<object>.Fail(
                    Array.Empty<string>(),
                    "Unauthorized access"
                );
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                payload = ApiResponse<object>.Fail(
                    new[] { "An internal server error occurred" },
                    "Server error"
                );
            }

            await response.WriteAsync(JsonSerializer.Serialize(payload, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
        }
    }
}