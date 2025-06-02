using System.ComponentModel.DataAnnotations;

namespace FIapCloud.App.Dtos;

public class AddGameToLibraryRequest
{
    [Required(ErrorMessage = "O ID do jogo é obrigatório.")]
    public Guid GameId { get; set; }
}
