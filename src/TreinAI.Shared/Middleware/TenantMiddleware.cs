using System.Security.Claims;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;

namespace TreinAI.Shared.Middleware;

/// <summary>
/// Azure Functions middleware that extracts tenant and user information
/// from JWT claims (Azure AD B2C) and populates TenantContext.
/// Falls back to X-Tenant-Id / X-User-Id headers for service-to-service calls.
/// </summary>
public class TenantMiddleware : IFunctionsWorkerMiddleware
{
    private readonly ILogger<TenantMiddleware> _logger;

    public TenantMiddleware(ILogger<TenantMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        var tenantContext = context.InstanceServices.GetService(typeof(TenantContext)) as TenantContext;
        if (tenantContext == null)
        {
            _logger.LogWarning("TenantContext not registered in DI. Skipping tenant extraction.");
            await next(context);
            return;
        }

        // Try to extract from ClaimsPrincipal (Azure AD B2C token)
        var claimsPrincipal = GetClaimsPrincipal(context);
        if (claimsPrincipal?.Identity?.IsAuthenticated == true)
        {
            tenantContext.UserId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? claimsPrincipal.FindFirst("oid")?.Value
                ?? string.Empty;

            tenantContext.Email = claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value
                ?? claimsPrincipal.FindFirst("emails")?.Value
                ?? string.Empty;

            tenantContext.UserName = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value
                ?? claimsPrincipal.FindFirst("name")?.Value
                ?? string.Empty;

            tenantContext.Role = claimsPrincipal.FindFirst(ClaimTypes.Role)?.Value
                ?? claimsPrincipal.FindFirst("extension_Role")?.Value
                ?? "aluno";

            tenantContext.TenantId = claimsPrincipal.FindFirst("extension_TenantId")?.Value
                ?? string.Empty;
        }

        // Fallback: extract from HTTP headers (for service-to-service or development)
        if (string.IsNullOrEmpty(tenantContext.TenantId) || string.IsNullOrEmpty(tenantContext.UserId))
        {
            var requestData = await context.GetHttpRequestDataAsync();
            if (requestData != null)
            {
                if (string.IsNullOrEmpty(tenantContext.TenantId) &&
                    requestData.Headers.TryGetValues("X-Tenant-Id", out var tenantValues))
                {
                    tenantContext.TenantId = tenantValues.FirstOrDefault() ?? string.Empty;
                }

                if (string.IsNullOrEmpty(tenantContext.UserId) &&
                    requestData.Headers.TryGetValues("X-User-Id", out var userValues))
                {
                    tenantContext.UserId = userValues.FirstOrDefault() ?? string.Empty;
                }

                if (string.IsNullOrEmpty(tenantContext.Role) &&
                    requestData.Headers.TryGetValues("X-User-Role", out var roleValues))
                {
                    tenantContext.Role = roleValues.FirstOrDefault() ?? "aluno";
                }
            }
        }

        if (string.IsNullOrEmpty(tenantContext.TenantId))
        {
            _logger.LogWarning("TenantId not found in claims or headers for function {FunctionName}",
                context.FunctionDefinition.Name);
        }

        _logger.LogDebug("Tenant context: TenantId={TenantId}, UserId={UserId}, Role={Role}",
            tenantContext.TenantId, tenantContext.UserId, tenantContext.Role);

        await next(context);
    }

    private static ClaimsPrincipal? GetClaimsPrincipal(FunctionContext context)
    {
        // Azure Functions isolated model exposes claims via Features
        if (context.Features.Get<ClaimsPrincipal>() is ClaimsPrincipal principal)
        {
            return principal;
        }

        return null;
    }
}
