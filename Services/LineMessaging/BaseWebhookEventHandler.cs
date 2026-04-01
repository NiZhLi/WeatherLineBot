using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WeatherBot.Dtos.Webhook;
using WeatherBot.Dtos.Webhook.SendMessage;

namespace WeatherBot.Services.LineMessaging
{
    /// <summary>
    /// Base abstract class for webhook event handlers providing common functionality
    /// </summary>
    public abstract class BaseWebhookEventHandler<TLogger> : IWebhookEventHandler where TLogger : class
    {
        protected readonly ILogger<TLogger> Logger;

        protected BaseWebhookEventHandler(ILogger<TLogger> logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public abstract bool CanHandle(WebhookEventDto webhookEvent);

        public async Task<RequestReplyMessageDto?> HandleAsync(WebhookEventDto webhookEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                Logger.LogDebug("Handling webhook event {EventId} of type {EventType}", 
                    webhookEvent.webhookEventId, webhookEvent.type);

                return await HandleEventAsync(webhookEvent, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error handling webhook event {EventId} of type {EventType}", 
                    webhookEvent.webhookEventId, webhookEvent.type);
                
                return CreateErrorReplyMessage(webhookEvent.replyToken);
            }
        }

        protected abstract Task<RequestReplyMessageDto?> HandleEventAsync(WebhookEventDto webhookEvent, CancellationToken cancellationToken);

        protected virtual RequestReplyMessageDto? CreateErrorReplyMessage(string replyToken)
        {
            return LineMessageBuilder.CreateReplyMessage(
                replyToken,
                LineMessageBuilder.CreateTextMessage("抱歉，處理您的請求時發生錯誤，請稍後再試。")
            );
        }

        protected RequestReplyMessageDto CreateTextReplyMessage(string replyToken, string text)
        {
            return LineMessageBuilder.CreateReplyMessage(
                replyToken,
                LineMessageBuilder.CreateTextMessage(text)
            );
        }

        protected string GetUserId(WebhookEventDto webhookEvent)
        {
            return webhookEvent.source?.userId ?? string.Empty;
        }
    }
}
