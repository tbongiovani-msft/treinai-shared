using Microsoft.Extensions.Logging;

namespace TreinAI.Shared.Services;

/// <summary>
/// Logging-based email service for development/testing.
/// Replace with AzureCommunicationEmailService when ACS infra is deployed (E1-07b, E3-30).
/// </summary>
public class LoggingEmailService : IEmailService
{
    private readonly ILogger<LoggingEmailService> _logger;

    public LoggingEmailService(ILogger<LoggingEmailService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
    {
        _logger.LogInformation(
            "[EMAIL] To: {To} | Subject: {Subject} | Body length: {Length} chars",
            to, subject, htmlBody.Length);

        _logger.LogDebug("[EMAIL BODY] {Body}", htmlBody);

        return Task.CompletedTask;
    }
}
