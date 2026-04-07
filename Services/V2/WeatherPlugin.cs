using System.ComponentModel;
using System.Text.Json;
using Microsoft.SemanticKernel;
using WeatherBot.Dtos.Domain;

namespace WeatherBot.Services.V2
{
    public class WeatherPlugin(DomainWeatherService domainWeatherService)
    {
        private readonly DomainWeatherService _domainWeatherService = domainWeatherService;

        [KernelFunction, Description("根據地點查詢今天(未來12小時)的天氣資訊")]
        public async Task<string> GetTodayWeatherAsync(
            [Description("台灣縣市，例如臺北市")]
            string location)
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                return "請提供要查詢的地點。";
            }

            var weather = await _domainWeatherService.Get12HrWeatherInfoAsync(DateTime.Now, location);

            if (weather == null || (!weather.TimePoints.Any() && !weather.TimePeriod.Any()))
            {
                return "無法取得天氣資訊。";
            }

            return JsonSerializer.Serialize(weather);
        }

        [KernelFunction, Description("根據地點查詢明天的天氣資訊")]
        public async Task<string> GetTomorrowWeatherAsync(
            [Description("台灣縣市，例如臺北市")]
            string location)
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                return "請提供要查詢的地點。";
            }

            var weather = await _domainWeatherService.GetTomorrowDaytimeWeatherInfoAsync(DateTime.Now, location);

            if (weather == null || (!weather.TimePoints.Any() && !weather.TimePeriod.Any()))
            {
                return "無法取得天氣資訊。";
            }

            return JsonSerializer.Serialize(weather);
        }
    }
}
