using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WeatherBot.Dtos.Webhook;
using WeatherBot.Services;
using WeatherBot.Services.LineMessaging;
using WeatherBot.Services.LineMessaging.UserPreferences;

namespace WeatherBot.Services.LineMessaging.Strategies
{
    public class KeywordReplyMessageStrategy(
        DomainWeatherService domainWeatherService,
        DomainMessageService domainMessageService,
        ITaiwanLocationResolver locationResolver,
        IUserPreferenceStore userPreferenceStore,
        ILocationChangeStateStore locationChangeStateStore) : IMessageStrategy
    {
        private readonly DomainWeatherService _domainWeatherService = domainWeatherService;
        private readonly DomainMessageService _domainMessageService = domainMessageService;
        private readonly ITaiwanLocationResolver _locationResolver = locationResolver;
        private readonly IUserPreferenceStore _userPreferenceStore = userPreferenceStore;
        private readonly ILocationChangeStateStore _locationChangeStateStore = locationChangeStateStore;

        private static readonly Dictionary<string, string> KeywordReplies = new(StringComparer.Ordinal)
        {
            ["明天天氣"] = "請輸入想查詢的縣市名稱，例如：臺北或新竹。",
            ["今天天氣"] = "目前僅支援明日白天的預報，請輸入縣市名稱取得資訊。",
            ["今天衣著建議"] = "目前僅支援「明天衣著建議」，可輸入：明天衣著建議 台北",
            ["明天衣著建議"] = "目前僅支援「明天衣著建議」，請輸入：明天衣著建議 台北",
            ["更改預設位置"] = "請輸入新的預設縣市名稱，例如：嘉義縣。"
        };

        public bool CanHandle(WebhookEventDto webhookEvent)
        {
            var messageText = webhookEvent.message?.text?.Trim();
            if (!IsTextMessage(webhookEvent) || string.IsNullOrEmpty(messageText))
            {
                return false;
            }

            var userId = webhookEvent.source?.userId;
            if (!string.IsNullOrWhiteSpace(userId) && _locationChangeStateStore.IsAwaitingLocationInput(userId))
            {
                return true;
            }

            return KeywordReplies.ContainsKey(messageText);
        }

        public async Task<string?> CreateReplyAsync(WebhookEventDto webhookEvent, CancellationToken cancellationToken = default)
        {
            var messageText = webhookEvent.message?.text?.Trim();
            if (string.IsNullOrEmpty(messageText))
            {
                return null;
            }

            var userId = webhookEvent.source?.userId;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return "無法識別使用者，請稍後再試。";
            }

            if (_locationChangeStateStore.IsAwaitingLocationInput(userId) &&
                !messageText.Equals("更改預設位置", StringComparison.Ordinal))
            {
                var normalizedLocation = _locationResolver.Resolve(messageText);
                await _userPreferenceStore.SetLocationAsync(userId, normalizedLocation, cancellationToken);
                _locationChangeStateStore.ClearAwaitingLocationInput(userId);

                return $"已預設{normalizedLocation}";
            }

            if (messageText.Equals("今天衣著建議"))
            {
                var preference = await _userPreferenceStore.GetByUserIdAsync(userId, cancellationToken);
                var preLocation = preference?.Location;

                if (preLocation == null)
                {
                    return "尚未設定預設位置，請先「更改預設位置」";
                }

                var location = _locationResolver.Resolve(preLocation);
                var weatherDetail = await _domainWeatherService.GetTodayDetailAsync(DateTime.UtcNow, location);
                return _domainMessageService.GetDressAdviceMessage(weatherDetail);
            }

            if (messageText.Equals("明天衣著建議"))
            {
                var preference = await _userPreferenceStore.GetByUserIdAsync(userId, cancellationToken);
                var preLocation = preference?.Location;

                if (preLocation == null)
                {
                    return "尚未設定預設位置，請先「更改預設位置」";
                }

                var location = _locationResolver.Resolve(preLocation);
                var weatherDetail = await _domainWeatherService.GetTomorrowDetailAsync(DateTime.UtcNow, location);
                return _domainMessageService.GetDressAdviceMessage(weatherDetail);
            }

            if (messageText.Equals("更改預設位置"))
            {
                _locationChangeStateStore.SetAwaitingLocationInput(userId);
                return "請輸入新的預設縣市名稱，例如：嘉義縣。";
            }

            if (messageText.Equals("今天天氣"))
            {
                var preference = await _userPreferenceStore.GetByUserIdAsync(userId, cancellationToken);
                var preLocation = preference?.Location;

                if (preLocation == null)
                {
                    return "尚未設定預設位置，請先「更改預設位置」";
                }

                var location = _locationResolver.Resolve(preLocation);
                var weatherDetail = await _domainWeatherService.GetTodayDetailAsync(DateTime.UtcNow, location);
                return _domainMessageService.GetWeatherMessage(weatherDetail);
            }

            if (messageText.Equals("明天天氣"))
            {
                var preference = await _userPreferenceStore.GetByUserIdAsync(userId, cancellationToken);
                var preLocation = preference?.Location;

                if (preLocation == null)
                {
                    return "尚未設定預設位置，請先「更改預設位置」";
                }

                var location = _locationResolver.Resolve(preLocation);
                var weatherDetail = await _domainWeatherService.GetTomorrowDetailAsync(DateTime.UtcNow, location);
                return _domainMessageService.GetWeatherMessage(weatherDetail);
            }

            KeywordReplies.TryGetValue(messageText, out var reply);
            return reply;
        }

        private static bool IsTextMessage(WebhookEventDto webhookEvent)
        {
            return string.Equals(webhookEvent.message?.type, "text", StringComparison.OrdinalIgnoreCase);
        }
    }
}
