using System.Threading;
using System.Threading.Tasks;
using WeatherBot.Dtos.Webhook;

namespace WeatherBot.Services.LineMessaging.Strategies
{
    public interface IMessageStrategy
    {
        bool CanHandle(WebhookEventDto webhookEvent);
        Task<string?> CreateReplyAsync(WebhookEventDto webhookEvent, CancellationToken cancellationToken = default);
    }
}
