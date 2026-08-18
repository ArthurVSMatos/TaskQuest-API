using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskQuest.API.Models;

[Table("tarefas")]
public class Tarefa
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("usuario_id")]
    public Guid UsuarioId { get; set; }

    [Column("categoria_id")]
    public Guid? CategoriaId { get; set; }

    [Required]
    [Column("titulo")]
    [MaxLength(150)]
    public string Titulo { get; set; } = string.Empty;

    [Column("descricao")]
    public string? Descricao { get; set; }

    [Required]
    [Column("data_vencimento")]
    public DateTime DataVencimento { get; set; }

    [Column("tempo_estimado_min")]
    public int TempoEstimadoMin { get; set; } = 30;

    [Column("tempo_real_min")]
    public int? TempoRealMin { get; set; }

    [Required]
    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "PENDENTE";

    [Required]
    [Column("prioridade")]
    [MaxLength(10)]
    public string Prioridade { get; set; } = "MEDIA";

    [Column("criado_em")]
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    [Column("concluido_em")]
    public DateTime? ConcluidoEm { get; set; }

    // Navegações
    public Usuario? Usuario { get; set; }
    public Categoria? Categoria { get; set; }
}