using System.ComponentModel.DataAnnotations;

namespace ControleDespesas.Models;

public class Usuario
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres")]
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;

    // Avoid serializing the full graph (Usuario -> Categorias -> Usuario ...)
    // The API should return DTOs when full related data is needed.
    [System.Text.Json.Serialization.JsonIgnore]
    public ICollection<Categoria> Categorias { get; set; } = new List<Categoria>();
}
