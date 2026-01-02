using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeatherBot.Services;
using WeatherBot.Dtos;

namespace WeatherBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController(WeatherOpenDataService weatherService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WeatherResponseDto>>> GetWeather([FromQuery] string city)
        {
          
            var weatherData = await weatherService.GetWeatherForecast36HrByTwLocationAsync(city);
            if (weatherData == null)
            {
                return BadRequest();
            }
            return Ok(weatherData);
        }
    }
}
