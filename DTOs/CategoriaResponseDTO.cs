namespace TaskQuest.API.DTOs;

public class CategoriaResponseDto
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public double MultiplicadorXp { get; set; }
    public string CorHex { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
}