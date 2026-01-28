using System;
using System.Threading;
using System.Threading.Tasks;
using WeatherBot.Dtos.Webhook;
using WeatherBot.Services;

namespace WeatherBot.Services.LineMessaging.Strategies
{
    public class LocationMessageStrategy(DomainWeatherService domainWeatherService) : IMessageStrategy
    {
        private readonly DomainWeatherService _domainWeatherService = domainWeatherService;

        public bool CanHandle(WebhookEventDto webhookEvent)
        {
            return string.Equals(webhookEvent.message?.type, "location", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<string?> CreateReplyAsync(WebhookEventDto webhookEvent, CancellationToken cancellationToken = default)
        {
            var userText = webhookEvent.message?.text;
            if (string.IsNullOrWhiteSpace(userText))
            {
                return "未能讀取到您的位置資訊，請輸入縣市名稱再試一次。";
            }

            return await _domainWeatherService.GetTomorrowWeatherInfoAsync(DateTime.UtcNow, userText);
        }
    }
}
