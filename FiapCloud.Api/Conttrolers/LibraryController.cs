using FIapCloud.App.Dtos;
using FIapCloud.App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

namespace FiapCloud.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LibraryController : ControllerBase
{
    private readonly ILibraryAppService _libraryAppService;

    public LibraryController(ILibraryAppService libraryAppService)
    {
        _libraryAppService = libraryAppService;
    }

    [HttpPost("add-game")]
    [Authorize(Policy = "PodeAcessarBiblioteca")]
    public async Task<IActionResult> AddGameToLibrary([FromBody] AddGameToLibraryRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }


        var applicationUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrEmpty(applicationUserIdString) || !Guid.TryParse(applicationUserIdString, out var applicationUserId))
        {
            return Unauthorized(new { message = "ID de usuário inválido no token." });
        }

        await _libraryAppService.AddGameToUserLibraryAsync(applicationUserId, request.GameId);


        return Ok(new { message = "Jogo adicionado à biblioteca com sucesso." });
    }

    [HttpGet]
    [Authorize(Policy = "PodeAcessarBiblioteca")]
    public async Task<ActionResult<IEnumerable<GameResponse>>> GetUserLibrary()
    {
        var applicationUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrEmpty(applicationUserIdString) || !Guid.TryParse(applicationUserIdString, out var applicationUserId))
        {
            return Unauthorized(new { message = "ID de usuário inválido no token." });
        }

        var gamesInLibrary = await _libraryAppService.GetGamesInUserLibraryAsync(applicationUserId);
        return Ok(gamesInLibrary);
    }
}