using System.ComponentModel.DataAnnotations;

namespace TaskQuest.API.DTOs;

public class CategoriaCreateDto
{
    [Required(ErrorMessage = "O nome da categoria é obrigatório.")]
    [StringLength(50, ErrorMessage = "O nome não pode exceder 50 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    public string? Descricao { get; set; }

    [Range(0.5, 3.0, ErrorMessage = "O multiplicador de XP deve estar entre 0.5x e 3.0x.")]
    public double MultiplicadorXp { get; set; } = 1.0;

    public string CorHex { get; set; } = "#4F46E5";
}