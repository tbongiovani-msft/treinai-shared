namespace TreinAI.Shared.Models;

public class PlanoNutricional : BaseEntity
{
    public string AlunoId { get; set; } = string.Empty;
    public string ProfessorId { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public DateTime DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public bool Ativo { get; set; } = true;
    public string? Objetivo { get; set; }
    public string? Orientacoes { get; set; }
    public MacronutrientesMeta? MetaDiaria { get; set; }
    public List<Refeicao> Refeicoes { get; set; } = [];
}

public class MacronutrientesMeta
{
    public double Calorias { get; set; }
    public double Proteinas { get; set; }
    public double Carboidratos { get; set; }
    public double Gorduras { get; set; }
}

public class Refeicao
{
    public string Nome { get; set; } = string.Empty; // e.g., "Café da manhã"
    public string? Horario { get; set; }
    public int Ordem { get; set; }
    public List<ItemRefeicao> Itens { get; set; } = [];
    public List<string>? Substituicoes { get; set; }
}

public class ItemRefeicao
{
    public string Alimento { get; set; } = string.Empty;
    public string? Quantidade { get; set; }
    public double? Calorias { get; set; }
    public double? Proteinas { get; set; }
    public double? Carboidratos { get; set; }
    public double? Gorduras { get; set; }
}
