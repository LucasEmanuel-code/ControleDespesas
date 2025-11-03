using System.ComponentModel.DataAnnotations;

namespace ControleDespesas.Models;

public class Transacao
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O valor é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
    public decimal Valor { get; set; }

    [Required(ErrorMessage = "A data é obrigatória")]
    [DataType(DataType.Date)]
    public DateTime Data { get; set; }

    [StringLength(200, ErrorMessage = "A descrição não pode exceder 200 caracteres")]
    public string? Descricao { get; set; }

    [Required(ErrorMessage = "O tipo é obrigatório")]
    [RegularExpression("^(Receita|Despesa)$", ErrorMessage = "O tipo deve ser 'Receita' ou 'Despesa'")]
    public string Tipo { get; set; } = string.Empty;

    [Required(ErrorMessage = "O usuário é obrigatório")]
    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    [Required(ErrorMessage = "A categoria é obrigatória")]
    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }
}