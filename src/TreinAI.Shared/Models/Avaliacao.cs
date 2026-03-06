namespace TreinAI.Shared.Models;

public class Avaliacao : BaseEntity
{
    public string AlunoId { get; set; } = string.Empty;
    public string ProfessorId { get; set; } = string.Empty;
    public DateTime DataAvaliacao { get; set; }

    // Composição corporal
    public double? Peso { get; set; }
    public double? Altura { get; set; }
    public double? PercentualGordura { get; set; }
    public double? MassaMagra { get; set; }
    public double? MassaGorda { get; set; }
    public double? Imc { get; set; }

    // Circunferências (cm)
    public Circunferencias? Circunferencias { get; set; }

    // Testes físicos
    public double? Flexibilidade { get; set; } // banco de Wells (cm)
    public Dictionary<string, int>? Rml { get; set; } // exercício -> repetições

    // Anamnese
    public string? Anamnese { get; set; }
    public string? Observacoes { get; set; }
}

public class Circunferencias
{
    public double? Pescoco { get; set; }
    public double? Ombro { get; set; }
    public double? Torax { get; set; }
    public double? BracoDireito { get; set; }
    public double? BracoEsquerdo { get; set; }
    public double? AntebracoDireito { get; set; }
    public double? AntebracoEsquerdo { get; set; }
    public double? Cintura { get; set; }
    public double? Abdomen { get; set; }
    public double? Quadril { get; set; }
    public double? CoxaDireita { get; set; }
    public double? CoxaEsquerda { get; set; }
    public double? PanturrilhaDireita { get; set; }
    public double? PanturrilhaEsquerda { get; set; }
}
