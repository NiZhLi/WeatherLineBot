using System.Threading;
using System.Threading.Tasks;
using WeatherBot.Dtos.Webhook;
using WeatherBot.Dtos.Webhook.SendMessage;

namespace WeatherBot.Services.LineMessaging
{
    public interface IWebhookEventHandler
    {
        bool CanHandle(WebhookEventDto webhookEvent);
        Task<RequestReplyMessageDto?> HandleAsync(WebhookEventDto webhookEvent, CancellationToken cancellationToken = default);
    }
}
