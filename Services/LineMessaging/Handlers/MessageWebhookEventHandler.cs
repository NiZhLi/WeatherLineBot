using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WeatherBot.Dtos.Webhook;
using WeatherBot.Dtos.Webhook.SendMessage;
using WeatherBot.Services.LineMessaging.Strategies;

namespace WeatherBot.Services.LineMessaging.Handlers
{
    public class MessageWebhookEventHandler : BaseWebhookEventHandler<MessageWebhookEventHandler>
    {
        private readonly IEnumerable<IMessageStrategy> _strategies;

        public MessageWebhookEventHandler(
            IEnumerable<IMessageStrategy> strategies, 
            ILogger<MessageWebhookEventHandler> logger) 
            : base(logger)
        {
            _strategies = strategies;
        }

        public override bool CanHandle(WebhookEventDto webhookEvent)
        {
            return string.Equals(webhookEvent.type, "message", StringComparison.OrdinalIgnoreCase);
        }

        protected override async Task<RequestReplyMessageDto?> HandleEventAsync(WebhookEventDto webhookEvent, CancellationToken cancellationToken)
        {
            var strategy = _strategies.FirstOrDefault(s => s.CanHandle(webhookEvent));
            if (strategy == null)
            {
                Logger.LogInformation("No message strategy registered for message type {MessageType}", webhookEvent.message?.type);
                return null;
            }

            var replyText = await strategy.CreateReplyAsync(webhookEvent, cancellationToken);
            if (string.IsNullOrWhiteSpace(replyText))
            {
                return null;
            }

            return CreateTextReplyMessage(webhookEvent.replyToken, replyText);
        }
    }
}
