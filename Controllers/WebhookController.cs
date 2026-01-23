using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WeatherBot.Dtos.Webhook;
using WeatherBot.Services;

namespace WeatherBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookController(LineBotService lineBotService, ILogger<WebhookController> logger) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> CreateWebhook(WebhookRequestDto webhookRequestDto)
        {
            //try
            //{
            //    await lineBotService.HandleWebhookAsync(webhookRequestDto);
            //}
            //catch (Exception ex)
            //{
            //    logger.LogError(ex, "Webhook 處理時發生錯誤: {ErrorMessage}", ex.Message);
            //    return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the webhook.");
            //}   
            
            return Ok();
        }
    }
}
