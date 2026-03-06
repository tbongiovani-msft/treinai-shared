namespace TreinAI.Shared.Models;

public class Objetivo : BaseEntity
{
    public string AlunoId { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime? DataLimite { get; set; }
    public bool Concluido { get; set; }
    public DateTime? DataConclusao { get; set; }
}
