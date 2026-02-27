using ControleDespesas.Data;
using ControleDespesas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ControleDespesas.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsuarioController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher<Usuario> _passwordHasher = new();

    public UsuarioController(AppDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId == null) return Unauthorized();

        return await _context.Usuarios.Where(u => u.Id == currentUserId.Value).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Usuario>> GetUsuario(int id)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId == null) return Unauthorized();

        if (id != currentUserId.Value) return Forbid();

        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null) return NotFound();
        return usuario;
    }

    // Register a new user (hashes password, enforces unique email)
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<Usuario>> Register([FromBody] Usuario usuario)
    {
        if (string.IsNullOrWhiteSpace(usuario.Email) || string.IsNullOrWhiteSpace(usuario.Senha))
        {
            return BadRequest("Email e senha são obrigatórios");
        }

        if (await _context.Usuarios.AnyAsync(u => u.Email == usuario.Email))
        {
            ModelState.AddModelError("Email", "Email já cadastrado");
            return BadRequest(ModelState);
        }

        // Hash password
        var plain = usuario.Senha;
        usuario.Senha = _passwordHasher.HashPassword(usuario, plain);

        usuario.DataCadastro = DateTime.UtcNow;
        usuario.IsActive = true;

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        // Do not return the password hash to client
        usuario.Senha = string.Empty;

        return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, usuario);
    }

    // Login: verifies credentials and returns basic user info on success
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<object>> Login([FromBody] LoginDto input)
    {
        if (string.IsNullOrWhiteSpace(input.Email) || string.IsNullOrWhiteSpace(input.Senha))
            return BadRequest("Email e senha são obrigatórios");

        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == input.Email);
        if (usuario == null)
            return Unauthorized("Usuário ou senha inválidos");

        var verify = _passwordHasher.VerifyHashedPassword(usuario, usuario.Senha, input.Senha);
        if (verify == PasswordVerificationResult.Success)
        {
            if (!usuario.IsActive)
                return Unauthorized("Usuário inativo");

            // Create claims and sign in using cookie authentication
            var claims = new List<System.Security.Claims.Claim>
            {
                new(System.Security.Claims.ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new(System.Security.Claims.ClaimTypes.Name, usuario.Nome),
                new(System.Security.Claims.ClaimTypes.Email, usuario.Email)
            };

            var identity = new System.Security.Claims.ClaimsIdentity(claims, Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new System.Security.Claims.ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Return minimal user info (without password)
            return Ok(new { usuario.Id, usuario.Nome, usuario.Email, usuario.DataCadastro, usuario.IsActive });
        }

        return Unauthorized("Usuário ou senha inválidos");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
    {
        if (id != usuario.Id)
            return BadRequest();

        // If password field provided, hash it before saving
        if (!string.IsNullOrWhiteSpace(usuario.Senha))
        {
            usuario.Senha = _passwordHasher.HashPassword(usuario, usuario.Senha);
        }
        var currentUserId = GetCurrentUserId();
        if (currentUserId == null) return Unauthorized();
        if (currentUserId.Value != id) return Forbid();

        _context.Entry(usuario).State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Usuarios.AnyAsync(u => u.Id == id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUsuario(int id)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId == null) return Unauthorized();
        if (currentUserId.Value != id) return Forbid();

        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null) return NotFound();

        _context.Usuarios.Remove(usuario);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // Logout: sign out the current user (cookie authentication)
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return NoContent();
    }

    // DTO used for login
    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }

    private int? GetCurrentUserId()
    {
        var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(id, out var value)) return value;
        return null;
    }
}