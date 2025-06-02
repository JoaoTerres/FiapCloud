// Em FiapCloud.Api/Conttrolers/UserController.cs
using FIapCloud.App.Dtos;
using FIapCloud.App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
// using System.Linq; // Removido se não for mais usado diretamente aqui

namespace FiapCloud.Api.Conttrolers; // ou FiapCloud.Api.Controllers

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserAppService _userAppService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserAppService userAppService, ILogger<UserController> logger)
    {
        _userAppService = userAppService;
        _logger = logger;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        _logger.LogInformation("Controller: POST /api/User - Criação de usuário {UserEmail}.", request.Email);
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var userId = await _userAppService.CreateUserAsync(request); 
        return Ok(new { id = userId });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
    {
        _logger.LogInformation("Controller: POST /api/User/login - Tentativa de login para {UserEmail}.", request.Email);
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var loginResponse = await _userAppService.LoginUserAsync(request);
        return Ok(loginResponse);
    }

    [HttpPatch("{applicationUserId}/status")]
    [Authorize(Policy = "PodeAdministrarUsuarios")]
    public async Task<IActionResult> ChangeStatus(Guid applicationUserId, [FromBody] ChangeUserStatusRequest request)
    {
        _logger.LogInformation("Controller: PATCH /api/User/{ApplicationUserId}/status - Alterar status para IsActive: {IsActive}.", applicationUserId, request.IsActive);
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var updatedAppUserId = await _userAppService.ChangeUserStatusAsync(applicationUserId, request);
        return Ok(new { id = updatedAppUserId, isActive = request.IsActive });
    }
}