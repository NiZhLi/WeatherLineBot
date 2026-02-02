using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using WeatherBot.Dtos.Webhook;
using WeatherBot.Dtos.Webhook.SendMessage;
using WeatherBot.Services.LineMessaging;

namespace WeatherBot.Services
{
    public class LineBotService(IConfiguration configuration, IHttpClientFactory httpClient, ILogger<LineBotService> logger, IEnumerable<IWebhookEventHandler> webhookEventHandlers)
    {
        private readonly string accessToken = configuration["LineWebhook:ChannelAccessToken"];
        private readonly string secret = configuration["LineWebhook:ChannelSecret"];
        private readonly string replyApiUrl = "https://api.line.me/v2/bot/message/reply";
        private readonly IEnumerable<IWebhookEventHandler> _webhookEventHandlers = webhookEventHandlers;

        public async Task HandleWebhookAsync(WebhookRequestDto webhookRequestDto, CancellationToken cancellationToken = default)
        {
            if (webhookRequestDto?.events == null || webhookRequestDto.events.Length == 0)
            {
                logger.LogInformation("No webhook events found in payload");
                return;
            }

            foreach (var webhookEvent in webhookRequestDto.events)
            {
                var handler = _webhookEventHandlers.FirstOrDefault(h => h.CanHandle(webhookEvent));
                if (handler == null)
                {
                    logger.LogInformation("Received unhandled webhook event type: {EventType}", webhookEvent.type);
                    continue;
                }

                var replyMessage = await handler.HandleAsync(webhookEvent, cancellationToken);
                if (replyMessage == null)
                {
                    continue;
                }

                await SendReplyAsync(replyMessage, cancellationToken);
            }
        }

        private async Task SendReplyAsync(RequestReplyMessageDto replyMessage, CancellationToken cancellationToken = default)
        {
            var client = httpClient.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, replyApiUrl)
            {
                Content = JsonContent.Create(replyMessage)
            };

            try
            {
                var response = await client.SendAsync(requestMessage, cancellationToken);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex, "HTTP request failed with content");
            }
        }

        private async Task SaveLocationToRedisAsync(string userId, object locationData)
        {
            // 假設已經有 Redis 客戶端實例化邏輯
            // 使用 Redis 儲存邏輯 (需根據實際 Redis 套件實作)
            // 例如：await redisClient.SetAsync($"user:location:{userId}", locationData);
                throw new NotImplementedException("請實作 Redis 儲存邏輯");
        }

    }
}
