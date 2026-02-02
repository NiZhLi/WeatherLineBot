using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WeatherBot.Dtos.Webhook;

namespace WeatherBot.Services.LineMessaging.Strategies
{
    public class KeywordReplyMessageStrategy : IMessageStrategy
    {
        private static readonly Dictionary<string, string> KeywordReplies = new(StringComparer.Ordinal)
        {
            ["明天天氣"] = "請輸入想查詢的縣市名稱，例如：臺北或新竹。",
            ["今天天氣"] = "目前僅支援明日白天的預報，請輸入縣市名稱取得資訊。",
            ["明天衣著建議"] = "衣著建議功能開發中，先提供天氣預報協助您決定穿搭。",
            ["今天衣著建議"] = "衣著建議功能開發中，請直接輸入縣市取得天氣資訊。",
            ["更改預設位置"] = "請分享新的位置或輸入縣市名稱，我會記住您的偏好。"
        };

        public bool CanHandle(WebhookEventDto webhookEvent)
        {
            var messageText = webhookEvent.message?.text?.Trim();
            return IsTextMessage(webhookEvent) && !string.IsNullOrEmpty(messageText) && KeywordReplies.ContainsKey(messageText);
        }

        public Task<string?> CreateReplyAsync(WebhookEventDto webhookEvent, CancellationToken cancellationToken = default)
        {
            var messageText = webhookEvent.message?.text?.Trim();
            if (string.IsNullOrEmpty(messageText))
            {
                return Task.FromResult<string?>(null);
            }

            KeywordReplies.TryGetValue(messageText, out var reply);
            return Task.FromResult<string?>(reply);
        }

        private static bool IsTextMessage(WebhookEventDto webhookEvent)
        {
            return string.Equals(webhookEvent.message?.type, "text", StringComparison.OrdinalIgnoreCase);
        }
    }
}
