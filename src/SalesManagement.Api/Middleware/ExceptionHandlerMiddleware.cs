using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SalesManagement.Api.Exceptions;

namespace SalesManagement.Api.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
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

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            var errorTitle = "Internal Server Error";
            var message = exception.Message;

            switch (exception)
            {
                case EmailAlreadyExistsException:
                    code = HttpStatusCode.Conflict;
                    errorTitle = "Email Already Exists";
                    break;
                case InvalidRefreshTokenException:
                    code = HttpStatusCode.Unauthorized;
                    errorTitle = "Invalid Refresh Token";
                    break;
                case RateLimitExceededException:
                    code = (HttpStatusCode)429; // Too Many Requests
                    errorTitle = "Too Many Requests";
                    break;
                case ArgumentException:
                case InvalidOperationException:
                    code = HttpStatusCode.BadRequest;
                    errorTitle = "Bad Request";
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            var result = JsonSerializer.Serialize(new
            {
                timestamp = DateTime.UtcNow,
                status = (int)code,
                error = errorTitle,
                message = message
            });

            return context.Response.WriteAsync(result);
        }
    }
}
