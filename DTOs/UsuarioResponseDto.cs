using System;

namespace TaskQuest.API.DTOs;

public class UsuarioResponseDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int XpTotal { get; set; }
    public int Nivel { get; set; }
    public DateTime CriadoEm { get; set; }
}