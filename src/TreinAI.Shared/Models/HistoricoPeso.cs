namespace TreinAI.Shared.Models;

/// <summary>
/// Tracks weight changes over time for an aluno.
/// Each time the aluno's Peso field is updated, a HistoricoPeso record is created.
/// </summary>
public class HistoricoPeso : BaseEntity
{
    public string AlunoId { get; set; } = string.Empty;
    public double PesoAnterior { get; set; }
    public double PesoNovo { get; set; }
    public DateTime DataRegistro { get; set; } = DateTime.UtcNow;
    public string? Observacao { get; set; }
}
