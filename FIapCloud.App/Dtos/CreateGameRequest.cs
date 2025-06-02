using System.ComponentModel.DataAnnotations;

namespace FIapCloud.App.Dtos;

public class CreateGameRequest
{
    [Required(ErrorMessage = "O nome do jogo é obrigatório.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Nome deve ter entre 1 e 100 caracteres.")]
    public string Name { get; set; } = default!;

    [Required(ErrorMessage = "A descrição é obrigatória.")]
    [StringLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres.")]
    public string Description { get; set; } = default!;

    [Range(0, double.MaxValue, ErrorMessage = "Preço não pode ser negativo.")]
    public decimal Price { get; set; }
}
