using System;

namespace TaskQuest.API.DTOs;

public class TarefaResponseDto
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public Guid? CategoriaId { get; set; }
    public string? CategoriaNome { get; set; }
    public string? CategoriaCorHex { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime DataVencimento { get; set; }
    public int TempoEstimadoMin { get; set; }
    public int? TempoRealMin { get; set; }
    public string Status { get; set; } = "PENDENTE";
    public string Prioridade { get; set; } = "MEDIA";

    // Regra da Tag Vermelha calculada em tempo de execução
    public bool IsAtrasada => Status != "CONCLUIDA" && DateTime.UtcNow > DataVencimento;

    public DateTime CriadoEm { get; set; }
    public DateTime? ConcluidoEm { get; set; }
}
