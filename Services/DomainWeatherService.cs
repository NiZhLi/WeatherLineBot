using MongoDB.Driver.Linq;
using WeatherBot.Dtos.Domain;
using WeatherBot.Dtos.Weather.v2;
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
        public async Task<WeatherDetailDto> GetTomorrowDetailAsync(DateTime nowDateTime, string location)
        {
            var tomorrowStartDate = nowDateTime.Date.AddDays(1);
            var timeFrom = tomorrowStartDate.AddHours(6); // 明天早上6點
            var timeTo = tomorrowStartDate.AddHours(18); // 明天晚上6點(溫度等只到17時)

            return await GetWeatherDetailInternalAsync(location, timeFrom, timeTo);
        }

        //今日查詢期間天氣預報(詳細)(至24時)
        public async Task<WeatherDetailDto> GetTodayDetailAsync(DateTime nowDateTime, string location)
        {
            var startDate = nowDateTime.Date;
            var timeTo = startDate.AddHours(24); // 今天晚上24點(溫度等只到23時)

            return await GetWeatherDetailInternalAsync(location, null, timeTo);
        }

        private async Task<WeatherDetailDto> GetWeatherDetailInternalAsync(string location, DateTime? timeFrom, DateTime? timeTo)
        {
            var element = new List<string> { "溫度", "相對濕度", "體感溫度", "蒲風級", "3小時降雨機率", "天氣現象" };

            // 使用 WeatherService 取得當天天氣資訊
            var Data = await _weatherService.ThreeDayDetailAsync(location, element, timeFrom, timeTo);

            // 提取天氣資訊
            var locationData = Data.records.Locations.FirstOrDefault()?.Location.FirstOrDefault();
            if (locationData == null)
            {
                return new WeatherDetailDto();
            }

            List<string> GetElementValues(string elementName, Func<Elementvalue, string?> selector)
            {
                return locationData.WeatherElement
                    .Where(e => e.ElementName == elementName)
                    .SelectMany(e => e.Time)
                    .Select(t => selector(t.ElementValue.FirstOrDefault()))
                    .Where(v => v != null)
                    .ToList()!;
            }

            // 各元素名對應氣象署api(domain：風速取蒲風級)
            var temperatureList = GetElementValues("溫度", ev => ev.Temperature);
            var humidityList = GetElementValues("相對濕度", ev => ev.RelativeHumidity);
            var apparentTempList = GetElementValues("體感溫度", ev => ev.ApparentTemperature);
            var beaufortList = GetElementValues("風速", ev => ev.BeaufortScale);
            var popList = GetElementValues("3小時降雨機率", ev => ev.ProbabilityOfPrecipitation);
            var weatherList = GetElementValues("天氣現象", ev => ev.Weather);

            List<double> ToDoubleList(List<string> values) => values
                .Select(v => double.TryParse(v, out var result) ? result : (double?)null)
                .Where(v => v.HasValue)
                .Select(v => v!.Value)
                .ToList();

            return new WeatherDetailDto
            {
                Temperatures = ToDoubleList(temperatureList),
                Humidities = ToDoubleList(humidityList),
                ApparentTemperatures = ToDoubleList(apparentTempList),
                BeaufortScales = ToDoubleList(beaufortList),
                PrecipitationProbabilities = popList,
                WeatherPhenomena = weatherList
            };
        }

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


        /*
         * version 2
         */

        // 時段判斷
        public async Task<WeatherDto> GetTodayWeatherInfoAsync(DateTime nowDateTime, string location)
        {
            DateTime timeFrom = nowDateTime;
            DateTime timeTo = nowDateTime;

            if (nowDateTime.Hour >= 6 && nowDateTime.Hour < 18) // 今日白天(06:00 - 18:00)
            {
                timeTo = nowDateTime.Date.AddHours(18); // 晚上6點(溫度等只到17時)
            }
            else if(nowDateTime.Hour >= 18) // 今晚明晨(18:00 - 06:00)
            {
                timeTo = nowDateTime.Date.AddDays(1).AddHours(6);
            }
            else if(nowDateTime.Hour < 6) // 今晨昨晚(18:00 - 06:00)，
            {
                timeTo = nowDateTime.Date.AddHours(6); // 今天早上6點
            }

            var element = new List<string> { "溫度", "相對濕度", "體感溫度", "風速", "3小時降雨機率", "天氣現象" };

            // 使用 WeatherService 取得當天天氣資訊
            var data = await _weatherService.ThreeDayDetailAsync(location, element, timeFrom, timeTo);

            // 把 map 工作交給獨立的方法
            var weatherDetail = WeatherDtoMap(data);

            if (weatherDetail.TimePoints == null || !weatherDetail.TimePoints.Any())
            {
                return null;
            }

            return weatherDetail;
        }

        // 明天白天 (06:00 - 18:00)
        public async Task<WeatherDto> GetTomorrowDaytimeWeatherInfoAsync(DateTime nowDateTime, string location)
        {
            var tomorrowStartDate = nowDateTime.Date.AddDays(1);
            var startTime = tomorrowStartDate.AddHours(6); // 明天早上6點
            var timeTo = tomorrowStartDate.AddHours(18); // 明天晚上6點(溫度等只到17時)

            var element = new List<string> { "溫度", "相對濕度", "體感溫度", "風速", "3小時降雨機率", "天氣現象" };

            // 使用 WeatherService 取得當天天氣資訊
            var data = await _weatherService.ThreeDayDetailAsync(location, element, startTime, timeTo);

            // 把 map 工作交給獨立的方法
            var weatherDetail = WeatherDtoMap(data);

            if (weatherDetail.TimePoints == null || !weatherDetail.TimePoints.Any())
            {
                return null;
            }

            return weatherDetail;
        }

        private WeatherDto WeatherDtoMap(TWDayDetailDto data)
        {
            // 提取天氣資訊
            var weatherElements = data.records.Locations.FirstOrDefault()?.Location.FirstOrDefault()?.WeatherElement;
            if (weatherElements == null)
            {
                return new WeatherDto { TimePoints = [] };
            }

            var allTimeRecords = weatherElements.SelectMany(we => we.Time.Select(t => new { we.ElementName, TimeData = t }));

            var pointRecords = allTimeRecords.Where(x => x.TimeData.DataTime != default).ToList();
            var periodRecords = allTimeRecords.Where(x => x.TimeData.StartTime != default).ToList();

            var points = pointRecords
                .GroupBy(x => x.TimeData.DataTime)
                .Select(g =>
                {
                    Elementvalue? GetElement(string name) => g.FirstOrDefault(x => x.ElementName == name)?.TimeData.ElementValue?.FirstOrDefault();
                    return new WeatherTimePointDto.PointElement
                    {
                        DataTime = g.Key,
                        Temperature = GetElement("溫度")?.Temperature ,
                        Humidity = GetElement("相對濕度")?.RelativeHumidity,
                        ApparentTemperature = GetElement("體感溫度")?.ApparentTemperature,
                        BeaufortScale = GetElement("風速")?.BeaufortScale
                    };
                })
                .OrderBy(p => p.DataTime)
                .ToList();

            var periods = periodRecords
                .GroupBy(x => new { x.TimeData.StartTime, x.TimeData.EndTime })
                .Select(g =>
                {
                    Elementvalue? GetElement(string name) => g.FirstOrDefault(x => x.ElementName == name)?.TimeData.ElementValue?.FirstOrDefault();
                    return new WeatherTimePointDto.PeriodElement
                    {
                        StartTime = g.Key.StartTime,
                        EndTime = g.Key.EndTime,
                        PrecipitationProbability = GetElement("3小時降雨機率")?.ProbabilityOfPrecipitation,
                        WeatherPhenomenon = GetElement("天氣現象")?.Weather
                    };
                })
                .OrderBy(p => p.StartTime)
                .ToList();

            return new WeatherDto 
            { 
                TimePoints = points,
                TimePeriod = periods
            };
        }

    }
}
