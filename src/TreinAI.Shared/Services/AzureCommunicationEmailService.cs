using Azure.Communication.Email;
using Microsoft.Extensions.Logging;

namespace TreinAI.Shared.Services;

/// <summary>
/// Email service implementation using Azure Communication Services.
/// Sends real emails via ACS Email. Replaces LoggingEmailService for production.
/// Requires ACS__ConnectionString and ACS__SenderAddress app settings.
/// </summary>
public class AzureCommunicationEmailService : IEmailService
{
    private readonly EmailClient _emailClient;
    private readonly string _senderAddress;
    private readonly ILogger<AzureCommunicationEmailService> _logger;

    public AzureCommunicationEmailService(
        string connectionString,
        string senderAddress,
        ILogger<AzureCommunicationEmailService> logger)
    {
        _emailClient = new EmailClient(connectionString);
        _senderAddress = senderAddress;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
    {
        _logger.LogInformation(
            "Sending email via ACS — To: {To}, Subject: {Subject}, Body length: {Length}",
            to, subject, htmlBody.Length);

        try
        {
            var emailMessage = new EmailMessage(
                senderAddress: _senderAddress,
                content: new EmailContent(subject) { Html = htmlBody },
                recipients: new EmailRecipients(
                    new List<EmailAddress> { new(to) }));

            var operation = await _emailClient.SendAsync(
                Azure.WaitUntil.Completed,
                emailMessage,
                ct);

            _logger.LogInformation(
                "Email sent successfully — To: {To}, OperationId: {OperationId}, Status: {Status}",
                to, operation.Id, operation.Value.Status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to send email via ACS — To: {To}, Subject: {Subject}",
                to, subject);
            throw;
        }
    }
}
