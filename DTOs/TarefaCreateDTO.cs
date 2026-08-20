using System;
using System.ComponentModel.DataAnnotations;

namespace TaskQuest.API.DTOs;

public class TarefaCreateDto
{
    [Required(ErrorMessage = "O título é obrigatório.")]
    [StringLength(150, ErrorMessage = "O título não pode exceder 150 caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    public string? Descricao { get; set; }

    public Guid? CategoriaId { get; set; }

    [Required(ErrorMessage = "A data de vencimento é obrigatória.")]
    public DateTime DataVencimento { get; set; }

    [Range(1, 1440, ErrorMessage = "O tempo estimado deve ser entre 1 e 1440 minutos.")]
    public int TempoEstimadoMin { get; set; } = 30;

    [RegularExpression("^(BAIXA|MEDIA|ALTA)$", ErrorMessage = "A prioridade deve ser BAIXA, MEDIA ou ALTA.")]
    public string Prioridade { get; set; } = "MEDIA";
}