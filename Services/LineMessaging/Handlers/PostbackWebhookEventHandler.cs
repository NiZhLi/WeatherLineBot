using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WeatherBot.Dtos.Webhook;
using WeatherBot.Dtos.Webhook.SendMessage;

namespace WeatherBot.Services.LineMessaging.Handlers
{
    /// <summary>
    /// Handles postback events from interactive components
    /// https://developers.line.biz/en/reference/messaging-api/#postback-event
    /// </summary>
    public class PostbackWebhookEventHandler : BaseWebhookEventHandler<PostbackWebhookEventHandler>
    {
        public PostbackWebhookEventHandler(ILogger<PostbackWebhookEventHandler> logger) 
            : base(logger)
        {
        }

        public override bool CanHandle(WebhookEventDto webhookEvent)
        {
            return string.Equals(webhookEvent.type, "postback", StringComparison.OrdinalIgnoreCase);
        }

        protected override Task<RequestReplyMessageDto?> HandleEventAsync(WebhookEventDto webhookEvent, CancellationToken cancellationToken)
        {
            // Postback data would be in webhookEvent.postback.data
            // Parse and handle based on your application logic
            
            Logger.LogInformation("Received postback event from user {UserId}", GetUserId(webhookEvent));

            // Example implementation - extend based on your needs
            return Task.FromResult<RequestReplyMessageDto?>(
                CreateTextReplyMessage(webhookEvent.replyToken, "收到您的回應！")
            );
        }
    }
}
