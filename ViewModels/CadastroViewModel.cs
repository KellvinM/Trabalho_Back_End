using System.ComponentModel.DataAnnotations;

namespace SistemaConsultasUVV.ViewModels;

public class CadastroViewModel
{
    [Required(ErrorMessage = "Informe o nome."), StringLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o e-mail."), EmailAddress(ErrorMessage = "E-mail inválido."), StringLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a senha."), StringLength(100, MinimumLength = 8, ErrorMessage = "A senha deve ter pelo menos 8 caracteres."), DataType(DataType.Password)]
    public string Senha { get; set; } = string.Empty;

    [Required, Compare(nameof(Senha), ErrorMessage = "As senhas não conferem."), DataType(DataType.Password)]
    public string ConfirmarSenha { get; set; } = string.Empty;
}
