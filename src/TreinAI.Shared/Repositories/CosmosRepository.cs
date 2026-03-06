using System.Linq.Expressions;
using System.Net;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;
using TreinAI.Shared.Models;

namespace TreinAI.Shared.Repositories;

/// <summary>
/// Generic Cosmos DB repository with partition key /tenantId.
/// Uses System.Text.Json serialization (configured in CosmosClient).
/// Implements soft-delete pattern.
/// </summary>
public class CosmosRepository<T> : IRepository<T> where T : BaseEntity
{
    private readonly Container _container;
    private readonly ILogger<CosmosRepository<T>> _logger;

    public CosmosRepository(Container container, ILogger<CosmosRepository<T>> logger)
    {
        _container = container;
        _logger = logger;
    }

    public async Task<T?> GetByIdAsync(string id, string tenantId, CancellationToken ct = default)
    {
        try
        {
            var response = await _container.ReadItemAsync<T>(
                id, new PartitionKey(tenantId), cancellationToken: ct);
            var item = response.Resource;
            return item is { IsDeleted: false } ? item : null;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(string tenantId, CancellationToken ct = default)
    {
        return await QueryAsync(tenantId, e => !e.IsDeleted, ct);
    }

    public async Task<IReadOnlyList<T>> QueryAsync(
        string tenantId,
        Expression<Func<T, bool>> predicate,
        CancellationToken ct = default)
    {
        var queryable = _container.GetItemLinqQueryable<T>(
                requestOptions: new QueryRequestOptions { PartitionKey = new PartitionKey(tenantId) })
            .Where(e => e.TenantId == tenantId && !e.IsDeleted)
            .Where(predicate);

        var results = new List<T>();
        using var iterator = queryable.ToFeedIterator();

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync(ct);
            _logger.LogDebug("Query consumed {RU} RU", response.RequestCharge);
            results.AddRange(response);
        }

        return results;
    }

    public async Task<(IReadOnlyList<T> Items, string? ContinuationToken)> GetPagedAsync(
        string tenantId,
        int pageSize,
        string? continuationToken = null,
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken ct = default)
    {
        var queryable = _container.GetItemLinqQueryable<T>(
                continuationToken: continuationToken,
                requestOptions: new QueryRequestOptions
                {
                    PartitionKey = new PartitionKey(tenantId),
                    MaxItemCount = pageSize
                })
            .Where(e => e.TenantId == tenantId && !e.IsDeleted);

        if (predicate is not null)
            queryable = queryable.Where(predicate);

        using var iterator = queryable.ToFeedIterator();

        if (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync(ct);
            _logger.LogDebug("Paged query consumed {RU} RU", response.RequestCharge);
            return (response.ToList(), response.ContinuationToken);
        }

        return ([], null);
    }

    public async Task<T> CreateAsync(T entity, CancellationToken ct = default)
    {
        entity.CreatedAt = DateTime.UtcNow;
        var response = await _container.CreateItemAsync(
            entity, new PartitionKey(entity.TenantId), cancellationToken: ct);
        _logger.LogDebug("Create consumed {RU} RU", response.RequestCharge);
        // response.Resource is null when EnableContentResponseOnWrite = false
        return response.Resource ?? entity;
    }

    public async Task<T> UpdateAsync(T entity, CancellationToken ct = default)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        var response = await _container.ReplaceItemAsync(
            entity, entity.Id, new PartitionKey(entity.TenantId), cancellationToken: ct);
        _logger.LogDebug("Update consumed {RU} RU", response.RequestCharge);
        // response.Resource is null when EnableContentResponseOnWrite = false
        return response.Resource ?? entity;
    }

    public async Task DeleteAsync(string id, string tenantId, CancellationToken ct = default)
    {
        var entity = await GetByIdAsync(id, tenantId, ct);
        if (entity is null) return;

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _container.ReplaceItemAsync(
            entity, entity.Id, new PartitionKey(tenantId), cancellationToken: ct);
    }

    public async Task<int> CountAsync(
        string tenantId,
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken ct = default)
    {
        var queryable = _container.GetItemLinqQueryable<T>(
                requestOptions: new QueryRequestOptions { PartitionKey = new PartitionKey(tenantId) })
            .Where(e => e.TenantId == tenantId && !e.IsDeleted);

        if (predicate is not null)
            queryable = queryable.Where(predicate);

        return await queryable.CountAsync(ct);
    }
}
