using WeatherBot.Dtos.Webhook;

namespace WeatherBot.Services
{
    public class DomainWeatherService
    {
        private readonly WeatherOpenDataService _weatherService;

        public DomainWeatherService(WeatherOpenDataService weatherService)
        {
            _weatherService = weatherService;
        }

        // 天氣
        public async Task<string> GetTomorrowWeatherInfoAsync(DateTime nowDateTime, string location)
        {
            var tomorrowStartDate = nowDateTime.Date.AddDays(1);
            var startTime = tomorrowStartDate.AddHours(6); // 明天早上6點

            // 使用 WeatherService 取得當天天氣資訊
            var weatherData = await _weatherService.GetWeatherForecast36HrByTwLocationAsync(location, startTime);
            var locationData = weatherData.records.location.FirstOrDefault();
            if (locationData == null)
            {
                return "無法獲取天氣資訊，請稍後再試。";
            }

            // 提取天氣資訊
            var weatherElement = locationData.weatherElement;
            var wx = weatherElement.FirstOrDefault(e => e.elementName == "Wx")?.time.FirstOrDefault()?.parameter.parameterName ?? "未知"; // 天氣狀況
            var pop = weatherElement.FirstOrDefault(e => e.elementName == "PoP")?.time.FirstOrDefault()?.parameter.parameterName ?? "未知"; // 降雨機率
            var minT = weatherElement.FirstOrDefault(e => e.elementName == "MinT")?.time.FirstOrDefault()?.parameter.parameterName ?? "未知"; // 最低溫
            var maxT = weatherElement.FirstOrDefault(e => e.elementName == "MaxT")?.time.FirstOrDefault()?.parameter.parameterName ?? "未知"; // 最高溫
            var ci = weatherElement.FirstOrDefault(e => e.elementName == "CI")?.time.FirstOrDefault()?.parameter.parameterName ?? "未知"; // 舒適度指數

            // 格式化成日期、最高溫、最低溫、天氣狀況、體感溫度 (溫度單位hard code)
            var formattedMessage = 
                $"{location}明日6-18點天氣預報" +
                $"天氣狀況：{wx}\n" +
                $"最高溫：{maxT}°C\n" +
                $"最低溫：{minT}°C\n" +
                $"體感溫度：{ci}" +
                $"降雨機率：{pop}%";

            return formattedMessage;
        }

        
    }
}
