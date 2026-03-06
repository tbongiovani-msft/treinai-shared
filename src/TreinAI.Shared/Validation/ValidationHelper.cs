using FluentValidation;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using System.Text.Json;
using TreinAI.Shared.Exceptions;

namespace TreinAI.Shared.Validation;

/// <summary>
/// Helper methods for running FluentValidation in Azure Functions.
/// </summary>
public static class ValidationHelper
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Deserializes the request body and validates it using the specified validator.
    /// Throws BusinessValidationException if validation fails.
    /// </summary>
    public static async Task<T> ValidateRequestAsync<T>(
        HttpRequestData request,
        IValidator<T> validator) where T : class
    {
        var body = await request.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(body))
        {
            throw new BusinessValidationException("Request body cannot be empty.");
        }

        T? model;
        try
        {
            model = JsonSerializer.Deserialize<T>(body, JsonOptions);
        }
        catch (JsonException ex)
        {
            throw new BusinessValidationException($"Invalid JSON format: {ex.Message}");
        }

        if (model == null)
        {
            throw new BusinessValidationException("Request body could not be deserialized.");
        }

        var result = await validator.ValidateAsync(model);
        if (!result.IsValid)
        {
            var errors = result.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray());

            throw new BusinessValidationException(errors);
        }

        return model;
    }

    /// <summary>
    /// Creates a JSON response with the specified status code and body.
    /// </summary>
    public static async Task<HttpResponseData> CreateJsonResponseAsync<T>(
        HttpRequestData request,
        HttpStatusCode statusCode,
        T body)
    {
        var response = request.CreateResponse(statusCode);
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        await response.WriteStringAsync(JsonSerializer.Serialize(body, JsonOptions));
        return response;
    }

    /// <summary>
    /// Creates a 200 OK response with JSON body.
    /// </summary>
    public static Task<HttpResponseData> OkAsync<T>(HttpRequestData request, T body) =>
        CreateJsonResponseAsync(request, HttpStatusCode.OK, body);

    /// <summary>
    /// Creates a 201 Created response with JSON body.
    /// </summary>
    public static Task<HttpResponseData> CreatedAsync<T>(HttpRequestData request, T body) =>
        CreateJsonResponseAsync(request, HttpStatusCode.Created, body);

    /// <summary>
    /// Creates a 204 No Content response.
    /// </summary>
    public static HttpResponseData NoContent(HttpRequestData request) =>
        request.CreateResponse(HttpStatusCode.NoContent);
}
