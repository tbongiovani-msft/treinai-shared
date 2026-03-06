namespace TreinAI.Shared.Models;

/// <summary>
/// Base entity for all Cosmos DB documents.
/// All entities use /tenantId as partition key for multi-tenancy.
/// </summary>
public abstract class BaseEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenantId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
}
