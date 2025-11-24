using AI_Marketplace.Application.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
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

            object errorResponse;
            if (exception is ValidationException validationEx)
            {
                errorResponse = new
                {
                    statusCode = (int)HttpStatusCode.BadRequest,
                    message = validationEx.Message,
                    errors = validationEx.Errors
                };
                response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            else if (exception is DuplicateOfferException duplicateEx)
            {
                errorResponse = new
                {
                    statusCode = (int)HttpStatusCode.Conflict,
                    message = duplicateEx.Message,
                    errors = new Dictionary<string, string[]>()
                };
                response.StatusCode = (int)HttpStatusCode.Conflict;
            }
            else if (exception is DbUpdateException dbEx && 
                     (dbEx.InnerException?.Message.Contains("IX_Offers_CustomRequestId_StoreId_Unique") == true ||
                      dbEx.InnerException?.Message.Contains("unique constraint", StringComparison.OrdinalIgnoreCase) == true))
            {
                _logger.LogWarning(dbEx, "Database unique constraint violation on Offers table");
                errorResponse = new
                {
                    statusCode = (int)HttpStatusCode.Conflict,
                    message = "Store has already submitted an offer for this custom request.",
                    errors = new Dictionary<string, string[]>()
                };
                response.StatusCode = (int)HttpStatusCode.Conflict;
            }
            else if (exception is UnauthorizedAccessException)
            {
                errorResponse = new
                {
                    statusCode = (int)HttpStatusCode.Unauthorized,
                    message = "Unauthorized access",
                    errors = new Dictionary<string, string[]>()
                };
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            else
            {
                errorResponse = new
                {
                    statusCode = (int)HttpStatusCode.InternalServerError,
                    message = "An internal server error occurred",
                    errors = new Dictionary<string, string[]>()
                };
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await response.WriteAsync(JsonSerializer.Serialize(errorResponse, jsonOptions));
        }
    }
}