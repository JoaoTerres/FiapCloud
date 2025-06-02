using System.ComponentModel.DataAnnotations;

namespace FIapCloud.App.Dtos;

public class CreatePromotionRequest
{
    [Required(ErrorMessage = "O nome da promoção é obrigatório.")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 200 caracteres.")]
    public string Name { get; set; } = default!;

    [Required(ErrorMessage = "A data inicial é obrigatória.")]
    public DateTime InitialDate { get; set; }

    [Required(ErrorMessage = "A data final é obrigatória.")]
    public DateTime FinalDate { get; set; }

    [Required(ErrorMessage = "O percentual de desconto é obrigatório.")]
    [Range(0.01, 100.00, ErrorMessage = "O percentual deve estar entre 0.01 e 100.00.")]
    public decimal Percentage { get; set; }

    public bool Enable { get; set; } = true;

    [Required(ErrorMessage = "Ao menos um ID de jogo deve ser fornecido.")]
    [MinLength(1, ErrorMessage = "A promoção deve ser aplicada a pelo menos um jogo.")]
    public List<Guid> GameIds { get; set; } = new();
}