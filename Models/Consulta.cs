using System.ComponentModel.DataAnnotations;

namespace SistemaConsultasUVV.Models;

public class Consulta
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Especialidade { get; set; } = string.Empty;

    [Required]
    public DateTime DataHora { get; set; }

    [Required, StringLength(1000)]
    public string Descricao { get; set; } = string.Empty;

    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
}
