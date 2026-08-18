using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskQuest.API.Models;

[Table("categorias")]
public class Categoria
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("usuario_id")]
    public Guid UsuarioId { get; set; }

    [Required]
    [Column("nome")]
    [MaxLength(50)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [Column("cor_hex")]
    [MaxLength(7)]
    public string CorHex { get; set; } = "#4F46E5";

    [Column("criado_em")]
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    // Navegação (Relacionamento com Usuário)
    public Usuario? Usuario { get; set; }
}