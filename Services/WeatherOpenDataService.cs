using System.Web;
using WeatherBot.Dtos;

namespace WeatherBot.Services
{
    public class WeatherOpenDataService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        private readonly string apiKey = configuration["ApiKey"];
        private const string baseUrl = $"https://opendata.cwa.gov.tw/api/v1/rest/datastore/F-C0032-001";
        public async Task<WeatherResponseDto> GetWeatherForecast36HrByTwLocationAsync(string locationZhTw)
        {
            var client = httpClientFactory.CreateClient();
            var RequestURL = $"{baseUrl}?Authorization={apiKey}&locationName={HttpUtility.UrlEncode(locationZhTw)}";
            var response = await client.GetAsync(RequestURL);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<WeatherResponseDto>();
                return result;
            }
            
            throw new Exception($"Status Code：{response.StatusCode}, Message：{await response.Content.ReadAsStringAsync()}");
        }


    }
}
