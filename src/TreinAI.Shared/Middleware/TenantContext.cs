namespace TreinAI.Shared.Middleware;

/// <summary>
/// Holds the current tenant context for the request.
/// Populated by the TenantMiddleware from JWT claims or headers.
/// </summary>
public class TenantContext
{
    public string TenantId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // "admin", "professor", "aluno"

    public bool IsAdmin => Role.Equals("admin", StringComparison.OrdinalIgnoreCase);
    public bool IsProfessor => Role.Equals("professor", StringComparison.OrdinalIgnoreCase);
    public bool IsAluno => Role.Equals("aluno", StringComparison.OrdinalIgnoreCase);
}
