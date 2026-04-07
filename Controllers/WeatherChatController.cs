using Microsoft.AspNetCore.Mvc;
using WeatherBot.Services.V2;

namespace WeatherBot.Controllers
{
    [ApiController]
    [Route("api/v2/[controller]")]
    public class WeatherChatController : ControllerBase
    {
        private readonly WeatherChatService _weatherChatService;

        public WeatherChatController(WeatherChatService weatherChatService)
        {
            _weatherChatService = weatherChatService;
        }


        [HttpPost("dress-suggestion")]
        public async Task<IActionResult> AskWeatherAsync([FromBody] string userInput)
        {
            var response = await _weatherChatService.ChatAsync(userInput);

            return Ok(new { Message = response });
        }
    }
}
