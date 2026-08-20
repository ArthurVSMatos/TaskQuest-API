using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskQuest.API.Models;

[Table("historico_xp")]
public class HistoricoXP
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("usuario_id")]
    public Guid UsuarioId { get; set; }

    [Column("tarefa_id")]
    public Guid? TarefaId { get; set; }

    [Required]
    [Column("xp_ganho")]
    public int XpGanho { get; set; }

    [Required]
    [Column("motivo")]
    [MaxLength(100)]
    public string Motivo { get; set; } = string.Empty;

    [Column("criado_em")]
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    // Navegações
    [ForeignKey("UsuarioId")]
    public Usuario? Usuario { get; set; }

    [ForeignKey("TarefaId")]
    public Tarefa? Tarefa { get; set; }
}
