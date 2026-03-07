namespace TreinAI.Shared.Services;

/// <summary>
/// Service to create in-app notifications for users.
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Creates a notification for the specified user.
    /// </summary>
    Task CreateAsync(string tenantId, string userId, string titulo, string mensagem,
        string tipo, string? linkUrl = null, string? createdBy = null);

    /// <summary>
    /// Creates notifications for multiple users.
    /// </summary>
    Task CreateForManyAsync(string tenantId, IEnumerable<string> userIds, string titulo,
        string mensagem, string tipo, string? linkUrl = null, string? createdBy = null);
}
