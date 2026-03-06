namespace TreinAI.Shared.Models;

public class Treino : BaseEntity
{
    public string AlunoId { get; set; } = string.Empty;
    public string ProfessorId { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public bool Ativo { get; set; } = true;
    public List<DivisaoTreino> Divisoes { get; set; } = [];
}

public class DivisaoTreino
{
    public string Nome { get; set; } = string.Empty; // e.g., "A", "B", "C"
    public string? Descricao { get; set; } // e.g., "Peito e Tríceps"
    public int Ordem { get; set; }
    public List<ExercicioTreino> Exercicios { get; set; } = [];
}

public class ExercicioTreino
{
    public string ExercicioId { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public int Ordem { get; set; }
    public int Series { get; set; }
    public string? Repeticoes { get; set; } // e.g., "12", "8-12", "até falha"
    public string? Carga { get; set; }
    public string? Metodo { get; set; } // e.g., "Drop-set", "Bi-set", "Rest-pause"
    public int? DescansoSegundos { get; set; }
    public string? LinkVideo { get; set; } // YouTube URL
    public string? Observacoes { get; set; }
}
