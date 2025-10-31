using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.WebApi.Common;
using FluentValidation;
using System.Net;
using System.Text.Json;

namespace Ambev.DeveloperEvaluation.WebApi.Middleware
{
    public class ValidationExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ValidationExceptionMiddleware> _logger;

        public ValidationExceptionMiddleware(RequestDelegate next, ILogger<ValidationExceptionMiddleware> logger)
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
            catch (ValidationException ex) 
            {
                await WriteResponseAsync(
                    context,
                    HttpStatusCode.BadRequest,
                    new ApiResponse
                    {
                        Success = false,
                        Message = "Validation Failed",
                        Errors = ex.Errors.Select(e => (ValidationErrorDetail)e)
                    });
            }
            catch (KeyNotFoundException ex) 
            {
                await WriteResponseAsync(
                    context,
                    HttpStatusCode.NotFound,
                    new ApiResponse
                    {
                        Success = false,
                        Message = ex.Message
                    });
            }
            catch (UnauthorizedAccessException ex) 
            {
                await WriteResponseAsync(
                    context,
                    HttpStatusCode.Unauthorized,
                    new ApiResponse
                    {
                        Success = false,
                        Message = ex.Message
                    });
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Unhandled error");
                await WriteResponseAsync(
                    context,
                    HttpStatusCode.InternalServerError,
                    new ApiResponse
                    {
                        Success = false,
                        Message = "Internal server error"
                    });
            }
        }

        private static Task WriteResponseAsync(HttpContext context, HttpStatusCode statusCode, ApiResponse body)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(body, jsonOptions));
        }
    }
}
