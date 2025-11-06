using ControleDespesas.Data;
using ControleDespesas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControleDespesas.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransacoesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<TransacoesController> _logger;
    private readonly IWebHostEnvironment _env;

    public TransacoesController(AppDbContext context, ILogger<TransacoesController> logger, IWebHostEnvironment env)
    {
        _context = context;
        _logger = logger;
        _env = env;
    }

    // DTO returned to clients to avoid serializing full EF entities and to include related data
    public class TransacaoDto
    {
        public int Id { get; set; }
        public decimal Valor { get; set; }
        public DateTime Data { get; set; }
        public string? Tipo { get; set; }
        public int CategoriaId { get; set; }
        public string? CategoriaNome { get; set; }
        public int UsuarioId { get; set; }
        public string? UsuarioEmail { get; set; }
        public string? Descricao { get; set; }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransacaoDto>>> GetTransacoes([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] int? categoriaId)
    {
        try
        {
            // Build base query including related data
            var query = _context.Transacoes
                .Include(t => t.Usuario)
                .Include(t => t.Categoria)
                .AsQueryable();

            // Apply optional filters
            if (startDate.HasValue)
            {
                // Compare from the start of the day (inclusive)
                var start = startDate.Value.Date;
                // Ensure UTC kind when comparing against timestamptz columns
                start = DateTime.SpecifyKind(start, DateTimeKind.Utc);
                query = query.Where(t => t.Data >= start);
            }

            if (endDate.HasValue)
            {
                // Make end inclusive by comparing to the next day (exclusive)
                var nextDay = endDate.Value.Date.AddDays(1);
                // Ensure UTC kind when comparing against timestamptz columns
                nextDay = DateTime.SpecifyKind(nextDay, DateTimeKind.Utc);
                query = query.Where(t => t.Data < nextDay);
            }

            if (categoriaId.HasValue && categoriaId.Value > 0)
            {
                query = query.Where(t => t.CategoriaId == categoriaId.Value);
            }

            var list = await query
                .Select(t => new TransacaoDto
                {
                    Id = t.Id,
                    Valor = t.Valor,
                    Data = t.Data,
                    Tipo = t.Tipo,
                    CategoriaId = t.CategoriaId,
                    CategoriaNome = t.Categoria != null ? t.Categoria.Nome : null,
                    UsuarioId = t.UsuarioId,
                    UsuarioEmail = t.Usuario != null ? t.Usuario.Email : null,
                    Descricao = t.Descricao
                })
                .ToListAsync();

            return list;
        }
        catch (Exception ex)
        {
            // Log and return a helpful error in Development, generic in Production
            _logger.LogError(ex, "Erro ao carregar transações com filtros startDate={StartDate} endDate={EndDate} categoriaId={CategoriaId}", startDate, endDate, categoriaId);
            if (_env.IsDevelopment())
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
            return StatusCode(500, "Erro ao carregar transações");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TransacaoDto>> GetTransacao(int id)
    {
        var t = await _context.Transacoes
            .Include(x => x.Usuario)
            .Include(x => x.Categoria)
            .Where(x => x.Id == id)
            .Select(x => new TransacaoDto
            {
                Id = x.Id,
                Valor = x.Valor,
                Data = x.Data,
                Tipo = x.Tipo,
                CategoriaId = x.CategoriaId,
                CategoriaNome = x.Categoria != null ? x.Categoria.Nome : null,
                UsuarioId = x.UsuarioId,
                UsuarioEmail = x.Usuario != null ? x.Usuario.Email : null,
                Descricao = x.Descricao
            })
            .FirstOrDefaultAsync();

        if (t == null) return NotFound();
        return t;
    }

    [HttpPost]
    public async Task<ActionResult<Transacao>> PostTransacao([FromBody] Transacao transacao)
    {
        // Validações manuais para campos não-nullable (previne 500 quando ausentes no JSON)
        if (transacao.Valor == 0)
        {
            ModelState.AddModelError("valor", "O valor é obrigatório e deve ser diferente de zero");
        }
        if (transacao.Data == default)
        {
            ModelState.AddModelError("data", "A data é obrigatória");
        }

        if (!ModelState.IsValid)
        {
            // Retornar ModelState com detalhes para facilitar depuração (Postman mostrará as chaves/erros)
            return BadRequest(ModelState);
        }

        // Validações de integridade referencial: garantir que FK apontem para registros existentes
        if (!await _context.Usuarios.AnyAsync(u => u.Id == transacao.UsuarioId))
        {
            ModelState.AddModelError("UsuarioId", "Usuário não encontrado");
            return BadRequest(ModelState);
        }

        if (!await _context.Categorias.AnyAsync(c => c.Id == transacao.CategoriaId))
        {
            ModelState.AddModelError("CategoriaId", "Categoria não encontrada");
            return BadRequest(ModelState);
        }

        // Normalize Data to UTC before saving.
        // Npgsql/PostgreSQL 'timestamp with time zone' requires UTC DateTime.Kind. If a
        // local or unspecified DateTime is written it throws the exception the client saw.
        if (transacao.Data.Kind == DateTimeKind.Local)
        {
            transacao.Data = transacao.Data.ToUniversalTime();
        }
        else if (transacao.Data.Kind == DateTimeKind.Unspecified)
        {
            // If unspecified, assume UTC (adjust if your app expects local times instead).
            transacao.Data = DateTime.SpecifyKind(transacao.Data, DateTimeKind.Utc);
        }

        _context.Transacoes.Add(transacao);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            // Log full exception for diagnostics
            _logger.LogError(ex, "Erro ao salvar transação (DbUpdateException)");

            // Try to map common PostgreSQL errors (requires Npgsql provider)
            var inner = ex.InnerException;
            if (inner != null && inner.GetType().FullName == "Npgsql.PostgresException")
            {
                // Use dynamic access to avoid compile-time dependency if type not available
                var sqlStateProp = inner.GetType().GetProperty("SqlState");
                var messageTextProp = inner.GetType().GetProperty("MessageText");
                var sqlState = sqlStateProp?.GetValue(inner) as string;
                var messageText = messageTextProp?.GetValue(inner) as string ?? inner.Message;

                // Foreign key violation
                if (sqlState == "23503")
                {
                    ModelState.AddModelError("ForeignKey", "Violação de integridade referencial: " + messageText);
                    return BadRequest(ModelState);
                }

                // Not-null violation
                if (sqlState == "23502")
                {
                    ModelState.AddModelError("NotNull", "Campo obrigatório ausente: " + messageText);
                    return BadRequest(ModelState);
                }

                // Unique violation
                if (sqlState == "23505")
                {
                    ModelState.AddModelError("Unique", "Violação de restrição única: " + messageText);
                    return BadRequest(ModelState);
                }
            }

            // If running in Development, return the inner exception message to help debugging
            if (_env.IsDevelopment())
            {
                var debugDetail = inner?.Message ?? ex.Message;
                return Problem(detail: "Erro ao salvar a transação no banco de dados. " + debugDetail, statusCode: 500);
            }

            // Generic fallback for Production
            return Problem(detail: "Erro ao salvar a transação no banco de dados.", statusCode: 500);
        }

        // Carrega as propriedades de navegação para retornar um objeto mais completo
        await _context.Entry(transacao).Reference(t => t.Usuario).LoadAsync();
        await _context.Entry(transacao).Reference(t => t.Categoria).LoadAsync();

        return CreatedAtAction(nameof(GetTransacao), new { id = transacao.Id }, transacao);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutTransacao(int id, Transacao transacao)
    {
        if (id != transacao.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _context.Entry(transacao).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Transacoes.AnyAsync(t => t.Id == id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTransacao(int id)
    {
        var transacao = await _context.Transacoes.FindAsync(id);
        if (transacao == null)
            return NotFound();

        _context.Transacoes.Remove(transacao);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}