namespace TreinAI.Shared.Models;

public class Usuario : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? B2CObjectId { get; set; }
    public string? PasswordHash { get; set; }
    public string Role { get; set; } = "aluno"; // "admin" | "professor" | "aluno"
    public bool Ativo { get; set; } = true;
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
    public string? ProfessorId { get; set; } // For aluno -> linked professor
    public string? AlunoId { get; set; } // Link to Aluno entity
}
