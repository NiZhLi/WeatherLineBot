using System;
using System.Threading;
using System.Threading.Tasks;
using WeatherBot.Dtos.Webhook;
using WeatherBot.Services;
using WeatherBot.Services.LineMessaging;

namespace WeatherBot.Services.LineMessaging.Strategies
{
    public class CityWeatherMessageStrategy(DomainWeatherService domainWeatherService, ITaiwanLocationResolver locationResolver) : IMessageStrategy
    {
        private readonly DomainWeatherService _domainWeatherService = domainWeatherService;
        private readonly ITaiwanLocationResolver _locationResolver = locationResolver;

        public bool CanHandle(WebhookEventDto webhookEvent)
        {
            return string.Equals(webhookEvent.message?.type, "text", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<string?> CreateReplyAsync(WebhookEventDto webhookEvent, CancellationToken cancellationToken = default)
        {
            var messageText = webhookEvent.message?.text;
            if (string.IsNullOrWhiteSpace(messageText))
            {
                return "請輸入想查詢的縣市名稱。";
            }

            var location = _locationResolver.Resolve(messageText);
            if (string.IsNullOrWhiteSpace(location))
            {
                return "未能識別縣市名稱，請重新輸入。";
            }

            return await _domainWeatherService.GetTomorrowWeatherInfoAsync(DateTime.UtcNow, location);
        }
    }
}
