using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using WeatherBot.Dtos.Weather.v1;
using WeatherBot.Dtos.Weather.v2;
using WeatherBot.Services;

namespace WeatherBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpenWeatherController(WeatherOpenDataService weatherService) : ControllerBase
    {
        [HttpGet("v1/36hr-weather")]
        public async Task<ActionResult<IEnumerable<WeatherResponseDto>>> Get36HRweather([FromQuery] string city)
        {
            var weatherData = await weatherService.GetWeatherForecast36HrByTwLocationAsync(city);
            if (weatherData == null)
            {
                return BadRequest();
            }
            return Ok(weatherData);
        }

        //[HttpGet("3days-weather")]
        //public async Task<ActionResult<TWDayDetailDto>> Get3DaysWeather([FromQuery] string city, [FromQuery] List<string> element, DateTime timeFrom, DateTime timeTo)
        //{
        //    var weatherData = await weatherService.ThreeDayDetailAsync(city, element, timeFrom, timeTo);
        //    if (weatherData == null)
        //    {
        //        return BadRequest();
        //    }
        //    return Ok(weatherData);
        //}

    }
}
