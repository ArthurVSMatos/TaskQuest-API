using System.Collections.Generic;

namespace TaskQuest.API.DTOs;

public class CategoriaEstatisticaDto
{
    public string CategoriaNome { get; set; } = string.Empty;
    public string CorHex { get; set; } = "#4F46E5";
    public int QuantidadeTarefas { get; set; }
}

public class RelatorioMensalDto
{
    public int TotalTarefasConcluidas { get; set; }
    public int TotalXpGanho { get; set; }
    public int TempoEstimadoTotalMin { get; set; }
    public int TempoRealTotalMin { get; set; }
    public List<CategoriaEstatisticaDto> TarefasPorCategoria { get; set; } = new();
}