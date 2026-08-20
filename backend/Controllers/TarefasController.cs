using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskQuest.API.Data;
using TaskQuest.API.DTOs;
using TaskQuest.API.Models;
using TaskQuest.API.Services;

namespace TaskQuest.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TarefasController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly GamificationService _gamificationService;

    public TarefasController(AppDbContext context, GamificationService gamificationService)
    {
        _context = context;
        _gamificationService = gamificationService;
    }

    private Guid ObterUsuarioIdLogado()
    {
        var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(usuarioIdClaim!);
    }

    // 1. GET: api/tarefas (Listar tarefas do usuário com filtro opcional por status)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TarefaResponseDto>>> GetTarefas([FromQuery] string? status)
    {
        var usuarioId = ObterUsuarioIdLogado();

        var query = _context.Tarefas
            .Include(t => t.Categoria)
            .Where(t => t.UsuarioId == usuarioId);

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(t => t.Status.ToUpper() == status.ToUpper());
        }

        var tarefas = await query
            .OrderByDescending(t => t.CriadoEm)
            .Select(t => new TarefaResponseDto
            {
                Id = t.Id,
                UsuarioId = t.UsuarioId,
                CategoriaId = t.CategoriaId,
                CategoriaNome = t.Categoria != null ? t.Categoria.Nome : null,
                CategoriaCorHex = t.Categoria != null ? t.Categoria.CorHex : null,
                Titulo = t.Titulo,
                Descricao = t.Descricao,
                DataVencimento = t.DataVencimento,
                TempoEstimadoMin = t.TempoEstimadoMin,
                TempoRealMin = t.TempoRealMin,
                Status = t.Status,
                Prioridade = t.Prioridade,
                CriadoEm = t.CriadoEm,
                ConcluidoEm = t.ConcluidoEm
            })
            .ToListAsync();

        return Ok(tarefas);
    }

    // 2. GET: api/tarefas/urgentes (Aba especial: atrasadas ou vencendo nas próximas 4 horas)
    [HttpGet("urgentes")]
    public async Task<ActionResult<IEnumerable<TarefaResponseDto>>> GetTarefasUrgentes()
    {
        var usuarioId = ObterUsuarioIdLogado();
        var agora = DateTime.UtcNow;
        var limite4Horas = agora.AddHours(4);

        var tarefasUrgentes = await _context.Tarefas
            .Include(t => t.Categoria)
            .Where(t => t.UsuarioId == usuarioId &&
                        t.Status != "CONCLUIDA" &&
                        (t.DataVencimento < agora || t.DataVencimento <= limite4Horas))
            .OrderBy(t => t.DataVencimento)
            .Select(t => new TarefaResponseDto
            {
                Id = t.Id,
                UsuarioId = t.UsuarioId,
                CategoriaId = t.CategoriaId,
                CategoriaNome = t.Categoria != null ? t.Categoria.Nome : null,
                CategoriaCorHex = t.Categoria != null ? t.Categoria.CorHex : null,
                Titulo = t.Titulo,
                Descricao = t.Descricao,
                DataVencimento = t.DataVencimento,
                TempoEstimadoMin = t.TempoEstimadoMin,
                TempoRealMin = t.TempoRealMin,
                Status = t.Status,
                Prioridade = t.Prioridade,
                CriadoEm = t.CriadoEm,
                ConcluidoEm = t.ConcluidoEm
            })
            .ToListAsync();

        return Ok(tarefasUrgentes);
    }

    // 3. POST: api/tarefas (Criar nova tarefa)
    [HttpPost]
    public async Task<ActionResult<TarefaResponseDto>> CreateTarefa(TarefaCreateDto dto)
    {
        var usuarioId = ObterUsuarioIdLogado();

        var tarefa = new Tarefa
        {
            UsuarioId = usuarioId,
            CategoriaId = dto.CategoriaId,
            Titulo = dto.Titulo,
            Descricao = dto.Descricao,
            DataVencimento = dto.DataVencimento.ToUniversalTime(),
            TempoEstimadoMin = dto.TempoEstimadoMin,
            Prioridade = dto.Prioridade.ToUpper(),
            Status = "PENDENTE"
        };

        _context.Tarefas.Add(tarefa);
        await _context.SaveChangesAsync();

        // Carregar a categoria para retornar os dados na resposta
        if (tarefa.CategoriaId.HasValue)
        {
            await _context.Entry(tarefa).Reference(t => t.Categoria).LoadAsync();
        }

        var response = new TarefaResponseDto
        {
            Id = tarefa.Id,
            UsuarioId = tarefa.UsuarioId,
            CategoriaId = tarefa.CategoriaId,
            CategoriaNome = tarefa.Categoria?.Nome,
            CategoriaCorHex = tarefa.Categoria?.CorHex,
            Titulo = tarefa.Titulo,
            Descricao = tarefa.Descricao,
            DataVencimento = tarefa.DataVencimento,
            TempoEstimadoMin = tarefa.TempoEstimadoMin,
            TempoRealMin = tarefa.TempoRealMin,
            Status = tarefa.Status,
            Prioridade = tarefa.Prioridade,
            CriadoEm = tarefa.CriadoEm,
            ConcluidoEm = tarefa.ConcluidoEm
        };

        return CreatedAtAction(nameof(GetTarefas), new { id = tarefa.Id }, response);
    }

    // 4. PATCH: api/tarefas/{id}/concluir (Apurar tempo real + Conceder XP + Atualizar Usuário)
    [HttpPatch("{id}/concluir")]
    public async Task<ActionResult<TarefaConcluidaResponseDto>> ConcluirTarefa(Guid id, TarefaConcluirDto dto)
    {
        var usuarioId = ObterUsuarioIdLogado();

        var tarefa = await _context.Tarefas
            .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);

        if (tarefa == null)
        {
            return NotFound(new { mensagem = "Tarefa não encontrada." });
        }

        if (tarefa.Status == "CONCLUIDA")
        {
            return BadRequest(new { mensagem = "Esta tarefa já foi concluída anteriormente." });
        }

        var usuario = await _context.Usuarios.FindAsync(usuarioId);
        if (usuario == null)
        {
            return NotFound(new { mensagem = "Usuário não encontrado." });
        }
        

        // Marcar a tarefa como concluída
        DateTime agora = DateTime.UtcNow;
        tarefa.Status = "CONCLUIDA";
        tarefa.TempoRealMin = dto.TempoRealMin;
        tarefa.ConcluidoEm = agora;
        

        // Calcular XP com base na Gamificação
        int xpGanho = _gamificationService.CalcularXpTarefa(tarefa.Prioridade, tarefa.DataVencimento, agora);

        // Processar ganho de XP e evolução de Nível do usuário
        var gamificationResult = _gamificationService.ProcessarGanhoXp(usuario, xpGanho);

        // Registrar no Histórico de XP para auditoria e gráficos
        var historicoXp = new HistoricoXP
        {
            UsuarioId = usuarioId,
            TarefaId = tarefa.Id,
            XpGanho = xpGanho,
            Motivo = $"Conclusão da tarefa: {tarefa.Titulo}"
        };

        _context.HistoricoXP.Add(historicoXp);
        await _context.SaveChangesAsync();

        return Ok(new TarefaConcluidaResponseDto
        {
            Mensagem = gamificationResult.SubiuDeNivel ? "🎉 PARABÉNS! Você subiu de nível!" : "Tarefa concluída com sucesso!",
            XpGanho = xpGanho,
            XpTotalAtual = gamificationResult.XpTotalAtual,
            NivelAtual = gamificationResult.NivelAtual,
            SubiuDeNivel = gamificationResult.SubiuDeNivel,
            TempoRealMin = dto.TempoRealMin
        });
    }

    // 5. DELETE: api/tarefas/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTarefa(Guid id)
    {
        var usuarioId = ObterUsuarioIdLogado();

        var tarefa = await _context.Tarefas
            .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);

        if (tarefa == null)
        {
            return NotFound(new { mensagem = "Tarefa não encontrada." });
        }

        _context.Tarefas.Remove(tarefa);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}