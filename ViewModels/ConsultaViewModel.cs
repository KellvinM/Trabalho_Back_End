using System.ComponentModel.DataAnnotations;

namespace SistemaConsultasUVV.ViewModels;

public class ConsultaViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe a especialidade."), StringLength(100)]
    public string Especialidade { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a data e hora."), Display(Name = "Data e hora"), DataType(DataType.DateTime)]
    public DateTime DataHora { get; set; }

    [Required(ErrorMessage = "Informe a descrição."), StringLength(1000)]
    public string Descricao { get; set; } = string.Empty;
}
