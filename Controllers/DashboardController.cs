using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskQuest.API.Data;
using TaskQuest.API.DTOs;

namespace TaskQuest.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    private Guid ObterUsuarioIdLogado()
    {
        var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(usuarioIdClaim!);
    }

    // GET: api/dashboard/relatorio-mensal
    [HttpGet("relatorio-mensal")]
    public async Task<ActionResult<RelatorioMensalDto>> GetRelatorioMensal()
    {
        var usuarioId = ObterUsuarioIdLogado();
        
        // Define o primeiro dia do mês atual
        var inicioDoMes = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        // Busca apenas as tarefas concluídas no mês corrente
        var tarefasConcluidasMes = await _context.Tarefas
            .Include(t => t.Categoria)
            .Where(t => t.UsuarioId == usuarioId && 
                        t.Status == "CONCLUIDA" && 
                        t.ConcluidoEm >= inicioDoMes)
            .ToListAsync();

        // Soma todo o XP recebido no mês
        var totalXpMes = await _context.HistoricoXP
            .Where(h => h.UsuarioId == usuarioId && h.CriadoEm >= inicioDoMes)
            .SumAsync(h => h.XpGanho);

        // Agrupa o total de tarefas por categoria
        var tarefasPorCategoria = tarefasConcluidasMes
            .GroupBy(t => t.Categoria != null ? t.Categoria.Nome : "Sem Categoria")
            .Select(g => new CategoriaEstatisticaDto
            {
                CategoriaNome = g.Key,
                CorHex = g.FirstOrDefault()?.Categoria?.CorHex ?? "#94A3B8",
                QuantidadeTarefas = g.Count()
            })
            .ToList();

        var relatorio = new RelatorioMensalDto
        {
            TotalTarefasConcluidas = tarefasConcluidasMes.Count,
            TotalXpGanho = totalXpMes,
            TempoEstimadoTotalMin = tarefasConcluidasMes.Sum(t => t.TempoEstimadoMin),
            TempoRealTotalMin = tarefasConcluidasMes.Sum(t => t.TempoRealMin ?? 0),
            TarefasPorCategoria = tarefasPorCategoria
        };

        return Ok(relatorio);
    }
}