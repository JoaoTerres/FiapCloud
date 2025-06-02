using FIapCloud.App.Dtos;
using FIapCloud.App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiapCloud.Api.Controllers;

[ApiController]
[Route("api/[controller]")]

public class PromotionController : ControllerBase
{
    private readonly IPromotionAppService _promotionAppService;

    public PromotionController(IPromotionAppService promotionAppService)
    {
        _promotionAppService = promotionAppService;
    }

    [HttpPost]
    [Authorize(Policy = "PodeCriarPromocoes")]
    public async Task<IActionResult> CreatePromotion([FromBody] CreatePromotionRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var promotionResponse = await _promotionAppService.CreatePromotionAsync(request);
        return Ok(promotionResponse);
    }

    [HttpGet]
    [Authorize(Policy = "PodeAcessarPlataforma")] 
    public async Task<ActionResult<IEnumerable<PromotionResponse>>> GetAllPromotions()
    {
        var promotions = await _promotionAppService.GetAllPromotionsAsync();
        return Ok(promotions);
    }

}