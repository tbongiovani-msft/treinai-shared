using System.Linq.Expressions;
using TreinAI.Shared.Models;

namespace TreinAI.Shared.Repositories;

/// <summary>
/// Generic repository interface for Cosmos DB operations.
/// All operations are scoped to a tenantId (partition key).
/// </summary>
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(string id, string tenantId, CancellationToken ct = default);
    Task<IReadOnlyList<T>> GetAllAsync(string tenantId, CancellationToken ct = default);
    Task<IReadOnlyList<T>> QueryAsync(string tenantId, Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task<(IReadOnlyList<T> Items, string? ContinuationToken)> GetPagedAsync(
        string tenantId,
        int pageSize,
        string? continuationToken = null,
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken ct = default);
    Task<T> CreateAsync(T entity, CancellationToken ct = default);
    Task<T> UpdateAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(string id, string tenantId, CancellationToken ct = default);
    Task<int> CountAsync(string tenantId, Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default);
}
