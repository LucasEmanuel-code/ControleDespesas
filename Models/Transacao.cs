using System.ComponentModel.DataAnnotations;

namespace ControleDespesas.Models;

public class Transacao
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O valor é obrigatório")]
    public decimal Valor { get; set; }

    [Required(ErrorMessage = "A data é obrigatória")]
    public DateTime Data { get; set; }

    public string? Descricao { get; set; }

    public string? Tipo { get; set; } // "Receita" ou "Despesa"

    [Required(ErrorMessage = "O usuário é obrigatório")]
    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    [Required(ErrorMessage = "A categoria é obrigatória")]
    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }
}