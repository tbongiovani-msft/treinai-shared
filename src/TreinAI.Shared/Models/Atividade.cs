namespace TreinAI.Shared.Models;

public class Atividade : BaseEntity
{
    public string AlunoId { get; set; } = string.Empty;
    public string TreinoId { get; set; } = string.Empty;
    public string DivisaoNome { get; set; } = string.Empty;
    public DateTime DataExecucao { get; set; }
    public int DuracaoMinutos { get; set; }
    public string? Observacoes { get; set; }
    public List<ExercicioExecutado> ExerciciosExecutados { get; set; } = [];
}

public class ExercicioExecutado
{
    public string ExercicioId { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public bool Concluido { get; set; }
    public List<SerieExecutada> Series { get; set; } = [];
    public string? Observacoes { get; set; }
}

public class SerieExecutada
{
    public int Numero { get; set; }
    public double? Carga { get; set; }
    public int? Repeticoes { get; set; }
    public bool Concluida { get; set; }
}
