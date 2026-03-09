using Azure.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TreinAI.Shared.Middleware;
using TreinAI.Shared.Repositories;
using TreinAI.Shared.Services;

namespace TreinAI.Shared.Extensions;

/// <summary>
/// Extension methods for IServiceCollection to register shared services.
/// Used by each Function App's Program.cs to configure DI.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers CosmosClient with DefaultAzureCredential (passwordless auth via Managed Identity).
    /// </summary>
    public static IServiceCollection AddCosmosDb(
        this IServiceCollection services,
        string cosmosEndpoint,
        string databaseName)
    {
        services.AddSingleton(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<CosmosClient>>();
            logger.LogInformation("Initializing CosmosClient for endpoint {Endpoint}", cosmosEndpoint);

            var options = new CosmosClientOptions
            {
                SerializerOptions = new CosmosSerializationOptions
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                },
                ConnectionMode = ConnectionMode.Direct,
                ApplicationName = "treinai",
                EnableContentResponseOnWrite = false // Save bandwidth on writes
            };

            return new CosmosClient(cosmosEndpoint, new DefaultAzureCredential(), options);
        });

        services.AddSingleton(sp =>
        {
            var client = sp.GetRequiredService<CosmosClient>();
            return client.GetDatabase(databaseName);
        });

        return services;
    }

    /// <summary>
    /// Registers IRepository<T> → CosmosRepository<T> for a specific container.
    /// </summary>
    public static IServiceCollection AddRepository<T>(
        this IServiceCollection services,
        string containerName) where T : Models.BaseEntity
    {
        services.AddScoped<IRepository<T>>(sp =>
        {
            var database = sp.GetRequiredService<Database>();
            var logger = sp.GetRequiredService<ILogger<CosmosRepository<T>>>();
            var container = database.GetContainer(containerName);
            return new CosmosRepository<T>(container, logger);
        });

        return services;
    }

    /// <summary>
    /// Registers TenantContext as scoped (one per request).
    /// </summary>
    public static IServiceCollection AddTenantContext(this IServiceCollection services)
    {
        services.AddScoped<TenantContext>();
        return services;
    }

    /// <summary>
    /// Convenience method to register all common shared services.
    /// Each Function App can call this once in Program.cs.
    /// </summary>
    public static IServiceCollection AddTreinAIShared(
        this IServiceCollection services,
        string cosmosEndpoint,
        string databaseName)
    {
        services.AddCosmosDb(cosmosEndpoint, databaseName);
        services.AddTenantContext();
        return services;
    }

    /// <summary>
    /// Registers INotificationService → NotificationService backed by the notificacoes container.
    /// Call AddRepository&lt;Notificacao&gt;("notificacoes") before this.
    /// </summary>
    public static IServiceCollection AddNotificationService(this IServiceCollection services)
    {
        services.AddScoped<INotificationService, NotificationService>();
        return services;
    }

    /// <summary>
    /// Registers the email service.
    /// If ACS__ConnectionString is set, uses Azure Communication Services for real email sending.
    /// Otherwise, falls back to LoggingEmailService for development/testing.
    /// </summary>
    public static IServiceCollection AddEmailService(this IServiceCollection services)
    {
        var connectionString = Environment.GetEnvironmentVariable("ACS__ConnectionString");
        var senderAddress = Environment.GetEnvironmentVariable("ACS__SenderAddress");

        if (!string.IsNullOrWhiteSpace(connectionString) && !string.IsNullOrWhiteSpace(senderAddress))
        {
            services.AddSingleton<IEmailService>(sp =>
            {
                var logger = sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<AzureCommunicationEmailService>>();
                return new AzureCommunicationEmailService(connectionString, senderAddress, logger);
            });
        }
        else
        {
            services.AddSingleton<IEmailService, LoggingEmailService>();
        }

        return services;
    }
}
