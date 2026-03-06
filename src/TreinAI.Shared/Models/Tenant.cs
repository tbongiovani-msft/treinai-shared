namespace TreinAI.Shared.Models;

public class Tenant : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public string Tipo { get; set; } = "personal"; // "personal" | "academy"
    public string? Cnpj { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public string AdminUserId { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
    public DateTime? DataExpiracao { get; set; }
}
