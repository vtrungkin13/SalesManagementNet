using Microsoft.AspNetCore.Diagnostics;
namespace Api.Middleware;

public static class ExceptionHandling
{
    public static void UseApiExceptionHandling(this WebApplication app)
    {
        app.UseExceptionHandler(handler => handler.Run(async context =>
        {
            var ex = context.Features.Get<IExceptionHandlerFeature>()?.Error;
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = ex switch { UnauthorizedAccessException => 401, KeyNotFoundException => 404, InvalidOperationException => 409, ArgumentException => 400, _ => 500 };
            await context.Response.WriteAsJsonAsync(new { error = new { code = $"HTTP_{context.Response.StatusCode}", message = ex?.Message ?? "An unexpected error occurred." } });
        }));
    }
}


