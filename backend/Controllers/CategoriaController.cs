using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskQuest.API.Data;
using TaskQuest.API.DTOs;
using TaskQuest.API.Models;

namespace TaskQuest.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriasController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriasController(AppDbContext context)
    {
        _context = context;
    }

    private Guid ObterUsuarioIdLogado()
    {
        var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(usuarioIdClaim!);
    }

    // 1. GET: api/categorias (Listar todas do usuário logado)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoriaResponseDto>>> GetCategorias()
    {
        var usuarioId = ObterUsuarioIdLogado();

        var categorias = await _context.Categorias
            .Where(c => c.UsuarioId == usuarioId)
            .Select(c => new CategoriaResponseDto
            {
                Id = c.Id,
                UsuarioId = c.UsuarioId,
                Nome = c.Nome,
                Descricao = c.Descricao,
                MultiplicadorXp = c.MultiplicadorXp,
                CorHex = c.CorHex,
                CriadoEm = c.CriadoEm
            })
            .ToListAsync();

        return Ok(categorias);
    }

    // 2. GET: api/categorias/{id} (Obter uma categoria específica)
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoriaResponseDto>> GetCategoriaPorId(Guid id)
    {
        var usuarioId = ObterUsuarioIdLogado();

        var categoria = await _context.Categorias
            .Where(c => c.Id == id && c.UsuarioId == usuarioId)
            .Select(c => new CategoriaResponseDto
            {
                Id = c.Id,
                UsuarioId = c.UsuarioId,
                Nome = c.Nome,
                Descricao = c.Descricao,
                MultiplicadorXp = c.MultiplicadorXp,
                CorHex = c.CorHex,
                CriadoEm = c.CriadoEm
            })
            .FirstOrDefaultAsync();

        if (categoria == null)
        {
            return NotFound(new { mensagem = "Categoria não encontrada." });
        }

        return Ok(categoria);
    }

    // 3. POST: api/categorias (Criar nova categoria)
    [HttpPost]
    public async Task<ActionResult<CategoriaResponseDto>> CreateCategoria(CategoriaCreateDto dto)
    {
        var usuarioId = ObterUsuarioIdLogado();

        var categoria = new Categoria
        {
            Nome = dto.Nome,
            Descricao = dto.Descricao,
            MultiplicadorXp = dto.MultiplicadorXp,
            CorHex = dto.CorHex,
            UsuarioId = usuarioId
        };

        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync();

        var response = new CategoriaResponseDto
        {
            Id = categoria.Id,
            UsuarioId = categoria.UsuarioId,
            Nome = categoria.Nome,
            Descricao = categoria.Descricao,
            MultiplicadorXp = categoria.MultiplicadorXp,
            CorHex = categoria.CorHex,
            CriadoEm = categoria.CriadoEm
        };

        return CreatedAtAction(nameof(GetCategoriaPorId), new { id = categoria.Id }, response);
    }

    // 4. PUT: api/categorias/{id} (Atualizar categoria existente)
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategoria(Guid id, CategoriaCreateDto dto)
    {
        var usuarioId = ObterUsuarioIdLogado();

        var categoria = await _context.Categorias
            .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);

        if (categoria == null)
        {
            return NotFound(new { mensagem = "Categoria não encontrada ou você não tem permissão para alterá-la." });
        }

        categoria.Nome = dto.Nome;
        categoria.Descricao = dto.Descricao;
        categoria.MultiplicadorXp = dto.MultiplicadorXp;
        categoria.CorHex = dto.CorHex;

        await _context.SaveChangesAsync();

        return NoContent(); // 204 No Content indica sucesso sem corpo na resposta
    }

    // 5. DELETE: api/categorias/{id} (Deletar categoria)
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategoria(Guid id)
    {
        var usuarioId = ObterUsuarioIdLogado();

        var categoria = await _context.Categorias
            .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);

        if (categoria == null)
        {
            return NotFound(new { mensagem = "Categoria não encontrada ou você não tem permissão para excluí-la." });
        }

        _context.Categorias.Remove(categoria);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}