using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WeatherBot.Dtos.Webhook;
using WeatherBot.Dtos.Webhook.SendMessage;
using WeatherBot.Services.LineMessaging.Strategies;

namespace WeatherBot.Services.LineMessaging.Handlers
{
    public class MessageWebhookEventHandler(IEnumerable<IMessageStrategy> strategies, ILogger<MessageWebhookEventHandler> logger) : IWebhookEventHandler
    {
        private readonly IEnumerable<IMessageStrategy> _strategies = strategies;
        private readonly ILogger<MessageWebhookEventHandler> _logger = logger;

        public bool CanHandle(WebhookEventDto webhookEvent)
        {
            return string.Equals(webhookEvent.type, "message", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<RequestReplyMessageDto?> HandleAsync(WebhookEventDto webhookEvent, CancellationToken cancellationToken = default)
        {
            var strategy = _strategies.FirstOrDefault(s => s.CanHandle(webhookEvent));
            if (strategy == null)
            {
                _logger.LogInformation("No message strategy registered for message type {MessageType}", webhookEvent.message?.type);
                return null;
            }

            var replyText = await strategy.CreateReplyAsync(webhookEvent, cancellationToken);
            if (string.IsNullOrWhiteSpace(replyText))
            {
                return null;
            }

            return new RequestReplyMessageDto
            {
                replyToken = webhookEvent.replyToken,
                messages = new List<Message>
                {
                    new Message
                    {
                        type = "text",
                        text = replyText
                    }
                }
            };
        }
    }
}
