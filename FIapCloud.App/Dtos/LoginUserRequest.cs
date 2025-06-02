using System.ComponentModel.DataAnnotations;

namespace FIapCloud.App.Dtos;

public class LoginUserRequest
{
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
    public string Email { get; set; } = default!;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    public string Password { get; set; } = default!;
}
