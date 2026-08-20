using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskQuest.API.Data;
using TaskQuest.API.DTOs;
using TaskQuest.API.Models;
using BCrypt.Net;

namespace TaskQuest.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsuariosController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/usuarios (PROTEGIDO COM JWT)
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<UsuarioResponseDto>>> GetUsuarios()
    {
        var usuarios = await _context.Usuarios
            .Select(u => new UsuarioResponseDto
            {
                Id = u.Id,
                Nome = u.Nome,
                Email = u.Email,
                XpTotal = u.XpTotal,
                Nivel = u.Nivel,
                CriadoEm = u.CriadoEm
            })
            .ToListAsync();

        return Ok(usuarios);
    }

    // POST: api/usuarios (PÚBLICO PARA CADASTRO)
    [HttpPost]
    public async Task<ActionResult<UsuarioResponseDto>> CreateUsuario(UsuarioCreateDto dto)
    {
        if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email))
        {
            return BadRequest("Este e-mail já está em uso.");
        }

        var usuario = new Usuario
        {
            Nome = dto.Nome,
            Email = dto.Email,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha)
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        var response = new UsuarioResponseDto
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            XpTotal = usuario.XpTotal,
            Nivel = usuario.Nivel,
            CriadoEm = usuario.CriadoEm
        };

        return CreatedAtAction(nameof(GetUsuarios), new { id = usuario.Id }, response);
    }
}