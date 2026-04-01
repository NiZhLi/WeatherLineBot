using Microsoft.AspNetCore.Mvc;
using WeatherBot.Dtos.Domain;
using WeatherBot.Services;

namespace WeatherBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DomainController(
        DomainWeatherService domainWeatherService,
        DomainMessageService domainMessageService) : ControllerBase
    {
        [HttpGet("today/detail")]
        public async Task<ActionResult<WeatherDetailDto>> GetTodayDetail([FromQuery] string city)
        {
            var weatherData = await domainWeatherService.GetTodayDetailAsync(DateTime.Now, city);
            if (!weatherData.Temperatures.Any())
            {
                return BadRequest("無法取得天氣資料");
            }

            return Ok(weatherData);
        }

        [HttpGet("tomorrow/detail")]
        public async Task<ActionResult<WeatherDetailDto>> GetTomorrowDetail([FromQuery] string city)
        {
            var weatherData = await domainWeatherService.GetTomorrowDetailAsync(DateTime.Now, city);
            if (!weatherData.Temperatures.Any())
            {
                return BadRequest("無法取得天氣資料");
            }

            return Ok(weatherData);
        }

        [HttpGet("today/message")]
        public async Task<ActionResult<string>> GetTodayMessage([FromQuery] string city)
        {
            var weatherData = await domainWeatherService.GetTodayDetailAsync(DateTime.Now, city);
            if (!weatherData.Temperatures.Any())
            {
                return BadRequest("無法取得天氣資料");
            }

            return Ok(domainMessageService.GetWeatherMessage(weatherData));
        }
        [HttpGet("tomorrow/message")]
        public async Task<ActionResult<string>> GetTomorrowMessage([FromQuery] string city)
        {
            var weatherData = await domainWeatherService.GetTomorrowDetailAsync(DateTime.Now, city);
            if (!weatherData.Temperatures.Any())
            {
                return BadRequest("無法取得天氣資料");
            }

            return Ok(domainMessageService.GetWeatherMessage(weatherData));
        }

        [HttpGet("today/dress-advice")]
        public async Task<ActionResult<string>> GetTodayDressAdvice([FromQuery] string city)
        {
            var weatherData = await domainWeatherService.GetTodayDetailAsync(DateTime.Now, city);
            if (!weatherData.Temperatures.Any())
            {
                return BadRequest("無法取得天氣資料");
            }

            return Ok(domainMessageService.GetDressAdviceMessage(weatherData));
        }

        [HttpGet("tomorrow/dress-advice")]
        public async Task<ActionResult<string>> GetTomorrowDressAdvice([FromQuery] string city)
        {
            var weatherData = await domainWeatherService.GetTomorrowDetailAsync(DateTime.Now, city);
            if (!weatherData.Temperatures.Any())
            {
                return BadRequest("無法取得天氣資料");
            }

            return Ok(domainMessageService.GetDressAdviceMessage(weatherData));
        }

    }
}
