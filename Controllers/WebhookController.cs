using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeatherBot.Dtos.Webhook;
using WeatherBot.Services;

namespace WeatherBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookController(LineBotService lineBotService) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> CreateWebhook(WebhookRequestDto webhookRequestDto)
        {
            //await lineBotService.HandleWebhookAsync(webhookRequestDto);
            return Ok();
        }
    }
}
