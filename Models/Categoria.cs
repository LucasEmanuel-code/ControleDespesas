using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ControleDespesas.Models;
public class Categoria
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome da categoria é obrigatório")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 50 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "A descrição não pode exceder 200 caracteres")]
    public string Descricao { get; set; } = string.Empty;

    [Required(ErrorMessage = "O usuário é obrigatório")]
    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    // Avoid serializing the circular navigation back to Categoria -> Transacoes -> Categoria ...
    [System.Text.Json.Serialization.JsonIgnore]
    public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
}