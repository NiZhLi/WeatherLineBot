using System.Web;
using System.Text.Json;
using WeatherBot.Dtos;
using WeatherBot.Dtos.Weather;

namespace WeatherBot.Services
{
    public class WeatherOpenDataService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<WeatherOpenDataService> logger)
    {
        private readonly string apiKey = configuration["ApiKey"];
        private const string BaseApiUrl = "https://opendata.cwa.gov.tw/api/v1/rest/datastore/";
        private const string TWDayDetailEndpoint = "F-D0047-089"; // 全台三天的天氣預報api的endpoint

        // 將方法改為泛型 <T> 以支援不同的 Dto
        public async Task<T> ResponseAsync<T>(string requestUrl)
        {
            var client = httpClientFactory.CreateClient();
            var response = await client.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    // 使用泛型 T 進行反序列化
                    var result = JsonSerializer.Deserialize<T>(responseContent);
                    return result ?? throw new JsonException("Deserialized result is null.");
                }
                catch (JsonException ex)
                {
                    logger.LogError(ex, "反序列化失敗: {ErrorMessage}", ex.Message);
                    throw new InvalidOperationException($"Failed to deserialize the response content. Error: {ex.Message}", ex);
                }
            }

            throw new Exception($"Status Code：{response.StatusCode}, Message：{await response.Content.ReadAsStringAsync()}");
        }

        // 一周的天氣預報api
        public async Task<WeatherResponseDto> OneWeekDetailAsync(string location, int limit, DateTime timeFrom, DateTime timeTo)
        {
            // 氣象署api startTime 參數型別 string
            string timeFromStr = timeFrom.ToString("yyyy-MM-ddTHH:mm:ss");
            string timeToStr = timeTo.ToString("yyyy-MM-ddTHH:mm:ss");
            
            var requestUrl = $"{BaseApiUrl}" +
                $"?Authorization={apiKey}" +
                $"&TimeFrom={timeFromStr}" +
                $"&TimeTo={timeToStr}" +
                $"&limit={limit}";

            // 等待並回傳泛型方法結果
            return await ResponseAsync<WeatherResponseDto>(requestUrl);
        }

        // 全台三天的天氣預報api
        public async Task<TWDayDetailDto> ThreeDayDetailAsync(string location, List<string>element, DateTime? timeFrom=null, DateTime? timeTo=null)
        {
            // 氣象署api startTime 參數型別 string
            string timeFromStr = timeFrom?.ToString("yyyy-MM-ddTHH:mm:ss") ?? "";
            string timeToStr = timeTo?.ToString("yyyy-MM-ddTHH:mm:ss") ?? "";

            var requestUrl = $"{BaseApiUrl}" +
                $"{TWDayDetailEndpoint}" +
                $"?Authorization={apiKey}" +
                $"&LocationName={location}" +
                $"&elementName={string.Join(",", element)}" +
                $"&timeFrom={timeFromStr}" +
                $"&timeTo={timeToStr}";

            // 等待並回傳泛型方法結果
            return await ResponseAsync<TWDayDetailDto>(requestUrl);
        }

        // 三十六小時的天氣預報api
        public async Task<WeatherResponseDto> GetWeatherForecast36HrByTwLocationAsync(string locationZhTw, DateTime? startTime= null)
        {
            string baseUrl = $"https://opendata.cwa.gov.tw/api/v1/rest/datastore/F-C0032-001";
            // 氣象暑api startTime 參數型別 string
            string startTimeStr = startTime?.ToString("yyyy-MM-ddTHH:mm:ss");

            var client = httpClientFactory.CreateClient();
            var RequestURL = $"{baseUrl}" +
                $"?Authorization={apiKey}" +
                $"&locationName={HttpUtility.UrlEncode(locationZhTw)}" +
                $"&startTime={startTimeStr}";
            var response = await client.GetAsync(RequestURL);

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<WeatherResponseDto>(responseContent);

                    return result;
                }
                catch (JsonException ex)
                {
                    logger.LogError(ex, "反序列化失敗: {ErrorMessage}", ex.Message);
                    throw new InvalidOperationException($"Failed to deserialize the response content. Error: {ex.Message}", ex);
                }

            }
            
            throw new Exception($"Status Code：{response.StatusCode}, Message：{await response.Content.ReadAsStringAsync()}");
        }


    }
}
