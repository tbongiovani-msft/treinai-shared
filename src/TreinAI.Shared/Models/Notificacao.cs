namespace TreinAI.Shared.Models;

public class Notificacao : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
    public string? Tipo { get; set; } // e.g., "novo_treino", "nova_avaliacao", "lembrete"
    public string? LinkUrl { get; set; }
    public bool Lida { get; set; }
    public DateTime? LidaEm { get; set; }
}
