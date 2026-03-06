namespace TreinAI.Shared.Services;

/// <summary>
/// Interface for sending email notifications.
/// Implementations can use Azure Communication Services, SendGrid, etc.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Send an email notification.
    /// </summary>
    Task SendEmailAsync(string to, string subject, string htmlBody, CancellationToken ct = default);
}
