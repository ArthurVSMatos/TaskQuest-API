using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskQuest.API.Data;
using TaskQuest.API.DTOs;
using BCrypt.Net;

namespace TaskQuest.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginDto dto)
    {
        // 1. Busca o usuário pelo e-mail
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (usuario == null)
        {
            return Unauthorized("E-mail ou senha inválidos.");
        }

        // 2. Valida a senha usando o BCrypt
        bool senhaValida = BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.SenhaHash);
        if (!senhaValida)
        {
            return Unauthorized("E-mail ou senha inválidos.");
        }

        // 3. Gera o Token JWT
        var token = GerarJwtToken(usuario.Id.ToString(), usuario.Email, usuario.Nome);

        return Ok(new LoginResponseDto
        {
            Token = token,
            Nome = usuario.Nome,
            Email = usuario.Email
        });
    }

    private string GerarJwtToken(string usuarioId, string email, string nome)
    {
        var secretKey = _configuration["JwtSettings:Secret"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuarioId),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim("nome", nome),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var expiracaoHoras = double.Parse(_configuration["JwtSettings:ExpiracaoEmHoras"] ?? "8");

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(expiracaoHoras),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}