namespace TreinAI.Shared.Exceptions;

/// <summary>
/// Thrown when a business rule validation fails.
/// Maps to HTTP 400 / 422.
/// </summary>
public class BusinessValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public BusinessValidationException(string message)
        : base(message)
    {
        Errors = new Dictionary<string, string[]>();
    }

    public BusinessValidationException(string message, IDictionary<string, string[]> errors)
        : base(message)
    {
        Errors = errors;
    }

    public BusinessValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }
}
