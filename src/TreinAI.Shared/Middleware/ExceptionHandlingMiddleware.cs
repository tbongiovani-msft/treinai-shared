using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;
using TreinAI.Shared.Exceptions;

namespace TreinAI.Shared.Middleware;

/// <summary>
/// Global exception handling middleware for Azure Functions.
/// Converts exceptions to RFC 7807 Problem Details responses.
/// </summary>
public class ExceptionHandlingMiddleware : IFunctionsWorkerMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception in {FunctionName}", context.FunctionDefinition.Name);

            var (statusCode, problemDetails) = MapExceptionToProblemDetails(ex, context);

            var httpRequest = await context.GetHttpRequestDataAsync();
            if (httpRequest != null)
            {
                var response = httpRequest.CreateResponse(statusCode);
                response.Headers.Add("Content-Type", "application/problem+json");
                await response.WriteStringAsync(JsonSerializer.Serialize(problemDetails, JsonOptions));

                // Set the invocation result to our error response
                var invocationResult = context.GetInvocationResult();
                invocationResult.Value = response;
            }
        }
    }

    private (HttpStatusCode statusCode, ProblemDetails details) MapExceptionToProblemDetails(
        Exception ex, FunctionContext context)
    {
        return ex switch
        {
            NotFoundException notFoundEx => (
                HttpStatusCode.NotFound,
                new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7807#section-3.1",
                    Title = "Resource Not Found",
                    Status = 404,
                    Detail = notFoundEx.Message,
                    Instance = context.FunctionDefinition.Name
                }),

            BusinessValidationException validationEx => (
                HttpStatusCode.UnprocessableEntity,
                new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7807#section-3.1",
                    Title = "Validation Error",
                    Status = 422,
                    Detail = validationEx.Message,
                    Instance = context.FunctionDefinition.Name,
                    Errors = validationEx.Errors
                }),

            ForbiddenException forbiddenEx => (
                HttpStatusCode.Forbidden,
                new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7807#section-3.1",
                    Title = "Forbidden",
                    Status = 403,
                    Detail = forbiddenEx.Message,
                    Instance = context.FunctionDefinition.Name
                }),

            ConflictException conflictEx => (
                HttpStatusCode.Conflict,
                new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7807#section-3.1",
                    Title = "Conflict",
                    Status = 409,
                    Detail = conflictEx.Message,
                    Instance = context.FunctionDefinition.Name
                }),

            UnauthorizedAccessException => (
                HttpStatusCode.Unauthorized,
                new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7807#section-3.1",
                    Title = "Unauthorized",
                    Status = 401,
                    Detail = "Authentication is required to access this resource.",
                    Instance = context.FunctionDefinition.Name
                }),

            _ => (
                HttpStatusCode.InternalServerError,
                new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7807#section-3.1",
                    Title = "Internal Server Error",
                    Status = 500,
                    Detail = "An unexpected error occurred. Please try again later.",
                    Instance = context.FunctionDefinition.Name
                })
        };
    }
}

/// <summary>
/// RFC 7807 Problem Details for HTTP APIs.
/// </summary>
public class ProblemDetails
{
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Status { get; set; }
    public string Detail { get; set; } = string.Empty;
    public string Instance { get; set; } = string.Empty;
    public IDictionary<string, string[]>? Errors { get; set; }
}
