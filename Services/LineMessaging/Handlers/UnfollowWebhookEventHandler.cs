using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WeatherBot.Dtos.Webhook;
using WeatherBot.Dtos.Webhook.SendMessage;

namespace WeatherBot.Services.LineMessaging.Handlers
{
    /// <summary>
    /// Handles unfollow events when a user blocks the bot
    /// https://developers.line.biz/en/reference/messaging-api/#unfollow-event
    /// </summary>
    public class UnfollowWebhookEventHandler : BaseWebhookEventHandler<UnfollowWebhookEventHandler>
    {
        public UnfollowWebhookEventHandler(ILogger<UnfollowWebhookEventHandler> logger) 
            : base(logger)
        {
        }

        public override bool CanHandle(WebhookEventDto webhookEvent)
        {
            return string.Equals(webhookEvent.type, "unfollow", StringComparison.OrdinalIgnoreCase);
        }

        protected override Task<RequestReplyMessageDto?> HandleEventAsync(WebhookEventDto webhookEvent, CancellationToken cancellationToken)
        {
            var userId = GetUserId(webhookEvent);
            Logger.LogInformation("User {UserId} unfollowed the bot", userId);

            // No reply message can be sent for unfollow events
            return Task.FromResult<RequestReplyMessageDto?>(null);
        }
    }
}
