using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WeatherBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookController(IConfiguration configuration) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> CreateWebHook()
        {
            // Handle the webhook payload here
            return Ok();
        }
    }
}
