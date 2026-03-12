using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WeatherBot.Dtos.Webhook;
using WeatherBot.Dtos.Webhook.SendMessage;

namespace WeatherBot.Services.LineMessaging
{
    public interface ILineBotService
    {
        Task HandleWebhookAsync(WebhookRequestDto webhookRequestDto, CancellationToken cancellationToken = default);
        Task SendReplyMessageAsync(RequestReplyMessageDto replyMessage, CancellationToken cancellationToken = default);
        Task SendPushMessageAsync(string userId, List<Message> messages, CancellationToken cancellationToken = default);
    }
}
