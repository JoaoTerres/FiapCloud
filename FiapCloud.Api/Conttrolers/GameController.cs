
using FiapCLoud.Domain.Exceptions;
using FIapCloud.App.Dtos;
using FIapCloud.App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiapCloud.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly IGameAppService _gameAppService;
    private readonly ILogger<GameController> _logger; 

    public GameController(IGameAppService gameAppService, ILogger<GameController> logger) 
    {
        _gameAppService = gameAppService;
        _logger = logger; 
    }

    [HttpPost]
    [Authorize(Policy = "PodeCadastrarJogos")]
    public async Task<IActionResult> Create([FromBody] CreateGameRequest request)
    {
        _logger.LogInformation("Controller: Recebida requisição para criar jogo: {GameName}", request.Name);
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Controller: ModelState inválido para a criação do jogo: {GameName}. Erros: {@ModelStateErrors}", request.Name, ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));
            return BadRequest(ModelState);
        }

        try
        {
            var gameId = await _gameAppService.CreateGameAsync(request);
            _logger.LogInformation("Controller: Jogo {GameName} criado com sucesso com ID: {GameId}", request.Name, gameId);
            return Ok(new { id = gameId }); 
        }
        catch (DomainException ex) 
        {
            _logger.LogWarning(ex, "Controller: DomainException ao criar jogo {GameName}: {ErrorMessage}. StatusCode: {StatusCode}", request.Name, ex.Message, ex.StatusCode);
            throw; 
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Controller: Erro inesperado ao criar jogo {GameName}.", request.Name);
            return StatusCode(500, "Ocorreu um erro interno ao processar a solicitação.");
        }
    }

    [HttpGet]
    [Authorize(Policy = "PodeAcessarPlataforma")]
    public async Task<ActionResult<IEnumerable<GameResponse>>> GetAll()
    {
        _logger.LogInformation("Controller: Recebida requisição para buscar todos os jogos.");
        try
        {
            var games = await _gameAppService.GetAllGamesAsync();
            _logger.LogInformation("Controller: Retornando {GameCount} jogos.", games.Count());
            return Ok(games);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Controller: Erro inesperado ao buscar todos os jogos.");
            return StatusCode(500, "Ocorreu um erro interno ao processar a solicitação.");
        }
    }
}