namespace TreinAI.Shared.Exceptions;

/// <summary>
/// Thrown when a requested entity is not found.
/// Maps to HTTP 404.
/// </summary>
public class NotFoundException : Exception
{
    public string EntityName { get; }
    public string EntityId { get; }

    public NotFoundException(string entityName, string entityId)
        : base($"{entityName} with id '{entityId}' was not found.")
    {
        EntityName = entityName;
        EntityId = entityId;
    }

    public NotFoundException(string message) : base(message)
    {
        EntityName = string.Empty;
        EntityId = string.Empty;
    }
}
