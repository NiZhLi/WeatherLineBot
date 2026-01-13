using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeatherBot.Services;
using WeatherBot.Dtos;

namespace WeatherBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpenWeatherController(WeatherOpenDataService weatherService) : ControllerBase
    {
        [HttpGet("v1")]
        public async Task<ActionResult<IEnumerable<WeatherResponseDto>>> Get36HRweather([FromQuery] string city)
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
