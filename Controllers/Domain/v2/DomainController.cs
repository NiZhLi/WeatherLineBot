using Microsoft.AspNetCore.Mvc;
using WeatherBot.Dtos.Domain;
using WeatherBot.Services;

namespace WeatherBot.Controllers.Domain.v2
{
    [Route("api/[controller]/v2")]
    [ApiController]
    public class DomainController(
        DomainWeatherService domainWeatherService,
        DomainMessageService domainMessageService) : ControllerBase
    {
        [HttpGet("weather/tomorrow/6am-to-6pm")]
        public async Task<ActionResult<WeatherDetailDto>> GetTomorrowWeather([FromQuery] string city)
        {
            var weatherData = await domainWeatherService.GetTomorrowDaytimeWeatherInfoAsync(DateTime.Now, city);
            if (weatherData == null)
            {
                return BadRequest("無法取得天氣資料");
            }

            return Ok(weatherData);
        }

        [HttpGet("weather/today/now-to-6am")]
        public async Task<ActionResult<WeatherDetailDto>> GetTodayWeather([FromQuery] string city)
        {
            var weatherData = await domainWeatherService.GetTodayWeatherInfoAsync(DateTime.Now, city);
            if (weatherData == null)
            {
                return BadRequest("無法取得天氣資料");
            }

            return Ok(weatherData);
        }

    }
}
