using System.ComponentModel.DataAnnotations;

namespace TaskQuest.API.DTOs;

public class TarefaConcluirDto
{
    [Required(ErrorMessage = "O tempo real investido em minutos é obrigatório.")]
    [Range(1, 1440, ErrorMessage = "O tempo real deve ser entre 1 e 1440 minutos.")]
    public int TempoRealMin { get; set; }
}