using Microsoft.Extensions.Logging;
using TreinAI.Shared.Models;
using TreinAI.Shared.Repositories;

namespace TreinAI.Shared.Services;

/// <summary>
/// Creates in-app notifications by writing directly to the notificacoes Cosmos container.
/// </summary>
public class NotificationService : INotificationService
{
    private readonly IRepository<Notificacao> _repo;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(IRepository<Notificacao> repo, ILogger<NotificationService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task CreateAsync(string tenantId, string userId, string titulo, string mensagem,
        string tipo, string? linkUrl = null, string? createdBy = null)
    {
        var notificacao = new Notificacao
        {
            Id = Guid.NewGuid().ToString(),
            TenantId = tenantId,
            UserId = userId,
            Titulo = titulo,
            Mensagem = mensagem,
            Tipo = tipo,
            LinkUrl = linkUrl,
            Lida = false,
            CreatedBy = createdBy ?? "system",
            UpdatedBy = createdBy ?? "system"
        };

        await _repo.CreateAsync(notificacao);
        _logger.LogInformation("Notification created: [{Tipo}] {Titulo} → user {UserId}", tipo, titulo, userId);
    }

    public async Task CreateForManyAsync(string tenantId, IEnumerable<string> userIds, string titulo,
        string mensagem, string tipo, string? linkUrl = null, string? createdBy = null)
    {
        foreach (var userId in userIds)
        {
            await CreateAsync(tenantId, userId, titulo, mensagem, tipo, linkUrl, createdBy);
        }
    }
}
