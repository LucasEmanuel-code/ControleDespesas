using System.ComponentModel.DataAnnotations;

namespace ControleDespesas.Models;

public class Usuario
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O email é obrigatório")]
    [EmailAddress(ErrorMessage = "O email informado não é válido")]
    [StringLength(100, ErrorMessage = "O email não pode exceder 100 caracteres")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 100 caracteres")]
    public string Senha { get; set; } = string.Empty;

    [System.Text.Json.Serialization.JsonIgnore]
    public ICollection<Categoria> Categorias { get; set; } = new List<Categoria>();
}
