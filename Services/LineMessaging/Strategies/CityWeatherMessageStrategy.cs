using System;
using System.Threading;
using System.Threading.Tasks;
using WeatherBot.Dtos.Webhook;
using WeatherBot.Services;
using WeatherBot.Services.LineMessaging;
using WeatherBot.Services.LineMessaging.UserPreferences;

namespace WeatherBot.Services.LineMessaging.Strategies
{
    public class CityWeatherMessageStrategy(
        DomainWeatherService domainWeatherService,
        ITaiwanLocationResolver locationResolver,
        IUserPreferenceStore userPreferenceStore) : IMessageStrategy
    {
        private readonly DomainWeatherService _domainWeatherService = domainWeatherService;
        private readonly ITaiwanLocationResolver _locationResolver = locationResolver;
        private readonly IUserPreferenceStore _userPreferenceStore = userPreferenceStore;

        public bool CanHandle(WebhookEventDto webhookEvent)
        {
            return string.Equals(webhookEvent.message?.type, "text", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<string?> CreateReplyAsync(WebhookEventDto webhookEvent, CancellationToken cancellationToken = default)
        {
            var messageText = webhookEvent.message?.text;
            var location = _locationResolver.Resolve(messageText);

            return await _domainWeatherService.GetTomorrowWeatherInfoAsync(DateTime.UtcNow, location);
        }

    }
}
