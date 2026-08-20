
using TaskQuest.API.Models;
using Microsoft.EntityFrameworkCore;


namespace TaskQuest.API.Data;

public class AppDbContext : DbContext
{
    // O construtor recebe as opções de conexão (que vamos configurar no Program.cs em breve)
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Aqui mapeamos cada Model para uma Tabela (DbSet) no banco de dados
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Tarefa> Tarefas { get; set; }
    public DbSet<HistoricoXP> HistoricoXP { get; set; }
}