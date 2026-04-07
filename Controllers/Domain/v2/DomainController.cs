using Microsoft.AspNetCore.Mvc;
using WeatherBot.Dtos.Domain;
using WeatherBot.Services;

namespace WeatherBot.Controllers.Domain.v2
{
    [Route("api/[controller]/v2")]
    [ApiController]
    public class DomainController(
        DomainWeatherService domainWeatherService) : ControllerBase
    {
        [HttpGet("weather/tomorrow/daytime")]
        public async Task<ActionResult<WeatherDetailDto>> GetTomorrowWeather([FromQuery] string city)
        {
            var weatherData = await domainWeatherService.GetTomorrowDaytimeWeatherInfoAsync(DateTime.Now, city);
            if (weatherData == null)
            {
                return BadRequest("無法取得天氣資料");
            }

            return Ok(weatherData);
        }

        [HttpGet("weather/today/auto-time-period")]
        public async Task<ActionResult<WeatherDetailDto>> GetPeriodWeather([FromQuery] string city)
        {
            var weatherData = await domainWeatherService.GetTodayWeatherInfoAsync(DateTime.Now, city);
            if (weatherData == null)
            {
                return BadRequest("無法取得天氣資料");
            }

            return Ok(weatherData);
        }

        [HttpGet("weather/today/12hr-later")]
        public async Task<ActionResult<WeatherDetailDto>> Get12HrWeather([FromQuery] string city)
        {
            var weatherData = await domainWeatherService.Get12HrWeatherInfoAsync(DateTime.Now, city);
            if (weatherData == null)
            {
                return BadRequest("無法取得天氣資料");
            }

            return Ok(weatherData);
        }

    }
}
