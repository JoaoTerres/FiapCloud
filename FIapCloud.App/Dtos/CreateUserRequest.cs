
using System.ComponentModel.DataAnnotations;

namespace FIapCloud.App.Dtos;

public class CreateUserRequest
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    public string Name { get; set; } = default!;

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
    public string Email { get; set; } = default!;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Senha deve ter entre 6 e 100 caracteres.")]
    [RegularExpression(
        @"^(?=.*[A-Z])(?=.*\d)(?=.*\W).+$",
        ErrorMessage = "A senha deve conter ao menos uma letra maiúscula, um número e um caractere especial."
    )]
    public string Password { get; set; } = default!;
}

