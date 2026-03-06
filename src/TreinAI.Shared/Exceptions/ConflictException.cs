namespace TreinAI.Shared.Exceptions;

/// <summary>
/// Thrown when a duplicate entity is detected (e.g., unique constraint violation).
/// Maps to HTTP 409.
/// </summary>
public class ConflictException : Exception
{
    public ConflictException(string message) : base(message)
    {
    }

    public ConflictException(string entityName, string field, string value)
        : base($"{entityName} with {field} '{value}' already exists.")
    {
    }
}
