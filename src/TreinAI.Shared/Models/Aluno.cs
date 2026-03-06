namespace TreinAI.Shared.Models;

public class Aluno : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public double? Peso { get; set; }
    public double? Altura { get; set; }
    public string? Objetivo { get; set; }
    public string? Observacoes { get; set; }
    public string? ProfessorId { get; set; }
    public string? UserId { get; set; } // Link to B2C user
    public string? FotoUrl { get; set; }
    public bool Ativo { get; set; } = true;
}
