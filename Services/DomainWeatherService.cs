using WeatherBot.Dtos.Webhook;

namespace WeatherBot.Services
{
    public class DomainWeatherService
    {
        private readonly WeatherOpenDataService _weatherService;
        private readonly ILogger<DomainWeatherService> _logger;

        public DomainWeatherService(WeatherOpenDataService weatherService)
        {
            _weatherService = weatherService;
        }


        //明6-晚間(18)天氣預報(詳細)
        public async Task<List<string>> GetTomorrowDetailAsync(DateTime nowDateTime, string location)
        {
            var tomorrowStartDate = nowDateTime.Date.AddDays(1);
            var timeFrom = tomorrowStartDate.AddHours(6); // 明天早上6點
            var timeTo = tomorrowStartDate.AddHours(18); // 明天晚上6點(溫度等只到17時)

            return await GetWeatherDetailInternalAsync(location, timeFrom, timeTo);
        }

        //今日查詢期間天氣預報(詳細)(至24時)
        public async Task<List<string>> GetTodayDetailAsync(DateTime nowDateTime, string location)
        {
            var startDate = nowDateTime.Date;
            var timeTo = startDate.AddHours(24); // 今天晚上24點(溫度等只到23時)

            return await GetWeatherDetailInternalAsync(location, null, timeTo);
        }

        private async Task<List<string>> GetWeatherDetailInternalAsync(string location, DateTime? timeFrom, DateTime? timeTo)
        {
            var element = new List<string> { "溫度", "相對濕度", "體感溫度", "蒲風級", "3小時降雨機率", "天氣現象" };

            // 使用 WeatherService 取得當天天氣資訊
            var Data = await _weatherService.ThreeDayDetailAsync(location, element, timeFrom, timeTo);

            // 提取天氣資訊
            var locationData = Data.records.Locations.FirstOrDefault()?.Location.FirstOrDefault();
            if (locationData == null)
            {
                return new List<string> { "無法取得該位置的天氣資訊資料。" };
            }

            List<string> GetElementValues(string elementName, Func<Dtos.Weather.Elementvalue, string?> selector)
            {
                return locationData.WeatherElement
                    .Where(e => e.ElementName == elementName)
                    .SelectMany(e => e.Time)
                    .Select(t => selector(t.ElementValue.FirstOrDefault()))
                    .Where(v => v != null)
                    .ToList()!;
            }

            var temperatureList = GetElementValues("溫度", ev => ev.Temperature);
            var humidityList = GetElementValues("相對濕度", ev => ev.RelativeHumidity);
            var apparentTempList = GetElementValues("體感溫度", ev => ev.ApparentTemperature);
            var beaufortList = GetElementValues("蒲風級", ev => ev.BeaufortScale);
            var popList = GetElementValues("3小時降雨機率", ev => ev.ProbabilityOfPrecipitation);
            var weatherList = GetElementValues("天氣現象", ev => ev.Weather);

            return new List<string>
            {
                $"溫度: {string.Join(", ", temperatureList)}",
                $"相對濕度: {string.Join(", ", humidityList)}",
                $"體感溫度: {string.Join(", ", apparentTempList)}",
                $"蒲風級: {string.Join(", ", beaufortList)}",
                $"3小時降雨機率: {string.Join(", ", popList)}",
                $"天氣現象: {string.Join(", ", weatherList)}"
            };
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
                $"體感溫度：{ci}\n" +
                $"降雨機率：{pop}%";

            return formattedMessage;
        }

        

    }
}
