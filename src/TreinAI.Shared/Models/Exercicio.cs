namespace TreinAI.Shared.Models;

public class Exercicio : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public string GrupoMuscular { get; set; } = string.Empty;
    public string? Equipamento { get; set; }
    public string? Descricao { get; set; }
    public string? LinkVideo { get; set; } // YouTube URL
    public string? ImagemUrl { get; set; }
    public List<string> Tags { get; set; } = [];
}
