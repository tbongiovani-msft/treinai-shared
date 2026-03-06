namespace TreinAI.Shared.Exceptions;

/// <summary>
/// Thrown when the user does not have permission for the requested operation.
/// Maps to HTTP 403.
/// </summary>
public class ForbiddenException : Exception
{
    public ForbiddenException()
        : base("You do not have permission to perform this action.")
    {
    }

    public ForbiddenException(string message) : base(message)
    {
    }
}
