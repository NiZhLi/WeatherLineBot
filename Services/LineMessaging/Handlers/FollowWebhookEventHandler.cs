using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WeatherBot.Dtos.Webhook;
using WeatherBot.Dtos.Webhook.SendMessage;

namespace WeatherBot.Services.LineMessaging.Handlers
{
    /// <summary>
    /// Handles follow events when a user adds the bot as a friend
    /// https://developers.line.biz/en/reference/messaging-api/#follow-event
    /// </summary>
    public class FollowWebhookEventHandler : BaseWebhookEventHandler<FollowWebhookEventHandler>
    {
        public FollowWebhookEventHandler(ILogger<FollowWebhookEventHandler> logger) 
            : base(logger)
        {
        }

        public override bool CanHandle(WebhookEventDto webhookEvent)
        {
            return string.Equals(webhookEvent.type, "follow", StringComparison.OrdinalIgnoreCase);
        }

        protected override Task<RequestReplyMessageDto?> HandleEventAsync(WebhookEventDto webhookEvent, CancellationToken cancellationToken)
        {
            var userId = GetUserId(webhookEvent);
            Logger.LogInformation("User {UserId} followed the bot", userId);

            var welcomeMessage = "歡迎使用天氣機器人！\n\n" +
                                "您可以：\n" +
                                "1. 傳送縣市名稱查詢天氣\n" +
                                "2. 傳送位置資訊取得當地天氣\n\n" +
                                "讓我們開始吧！";

            return Task.FromResult<RequestReplyMessageDto?>(
                CreateTextReplyMessage(webhookEvent.replyToken, welcomeMessage)
            );
        }
    }
}
