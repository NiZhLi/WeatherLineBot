using System.Web;
using System.Text.Json;
using WeatherBot.Dtos;

namespace WeatherBot.Services
{
    public class WeatherOpenDataService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<WeatherOpenDataService> logger)
    {
        private readonly string apiKey = configuration["ApiKey"];
        private const string baseUrl = $"https://opendata.cwa.gov.tw/api/v1/rest/datastore/F-C0032-001";
        public async Task<WeatherResponseDto> GetWeatherForecast36HrByTwLocationAsync(string locationZhTw, DateTime? startTime= null)
        {
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
