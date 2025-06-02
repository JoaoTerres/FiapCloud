using System.ComponentModel.DataAnnotations;

namespace FIapCloud.App.Dtos;

public class ChangeUserStatusRequest
{
    [Required(ErrorMessage = "O status de ativação é obrigatório.")]
    public bool IsActive { get; set; }
}
