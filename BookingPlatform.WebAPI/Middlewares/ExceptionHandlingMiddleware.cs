using BookingPlatform.Core.Exceptions;

namespace BookingPlatform.WebAPI.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        _logger.LogWarning("Middleware started");

        try
        {
            await _next(context);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Not Found");
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await WriteErrorAsync(context, ex.Message);
        }
        catch (ConflictException ex)
        {
            _logger.LogWarning(ex, "Conflict");
            context.Response.StatusCode = StatusCodes.Status409Conflict;
            await WriteErrorAsync(context, ex.Message);
        }
        catch (EmailAlreadyExistsException ex)
        {
            _logger.LogWarning(ex, "Email Already Exists");
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await WriteErrorAsync(context, ex.Message);
        }
        catch (CredentialsNotValidException ex)
        {
            _logger.LogWarning(ex, "Invalid Credentials");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await WriteErrorAsync(context, ex.Message);
        }
        catch (ForbiddenAccessException ex)
        {
            _logger.LogWarning(ex, "Forbidden");
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await WriteErrorAsync(context, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await WriteErrorAsync(context, "An unexpected error occurred.");
        }
    }

    private async Task WriteErrorAsync(HttpContext context, string message)
    {
        context.Response.ContentType = "application/json";
        var errorResponse = new { error = message };
        await context.Response.WriteAsJsonAsync(errorResponse);
    }
}



