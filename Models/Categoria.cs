namespace ControleDespesas.Models;
public class Categoria
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;

    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    // Avoid serializing the circular navigation back to Categoria -> Transacoes -> Categoria ...
    [System.Text.Json.Serialization.JsonIgnore]
    public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
}