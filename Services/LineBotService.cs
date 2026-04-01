using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WeatherBot.Dtos.Webhook;
using WeatherBot.Dtos.Webhook.SendMessage;
using WeatherBot.Services.LineMessaging;

namespace WeatherBot.Services
{
    public class LineBotService(
        IConfiguration configuration, 
        IHttpClientFactory httpClientFactory, 
        ILogger<LineBotService> logger, 
        IEnumerable<IWebhookEventHandler> webhookEventHandlers) : ILineBotService
    {
        private readonly string _accessToken = configuration["LineWebhook:ChannelAccessToken"] 
            ?? throw new InvalidOperationException("LineWebhook:ChannelAccessToken is not configured");
        private readonly string _replyApiUrl = "https://api.line.me/v2/bot/message/reply";
        private readonly string _pushApiUrl = "https://api.line.me/v2/bot/message/push";
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly ILogger<LineBotService> _logger = logger;
        private readonly IEnumerable<IWebhookEventHandler> _webhookEventHandlers = webhookEventHandlers;

        public async Task HandleWebhookAsync(WebhookRequestDto webhookRequestDto, CancellationToken cancellationToken = default)
        {
            if (webhookRequestDto?.events == null || webhookRequestDto.events.Length == 0)
            {
                _logger.LogInformation("No webhook events found in payload");
                return;
            }

            _logger.LogInformation("Processing {EventCount} webhook events", webhookRequestDto.events.Length);

            var tasks = webhookRequestDto.events.Select(async webhookEvent =>
            {
                try
                {
                    await ProcessWebhookEventAsync(webhookEvent, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process webhook event {EventId} of type {EventType}", 
                        webhookEvent.webhookEventId, webhookEvent.type);
                }
            });

            await Task.WhenAll(tasks);
        }

        private async Task ProcessWebhookEventAsync(WebhookEventDto webhookEvent, CancellationToken cancellationToken)
        {
            var handler = _webhookEventHandlers.FirstOrDefault(h => h.CanHandle(webhookEvent));
            if (handler == null)
            {
                _logger.LogInformation("No handler registered for webhook event type: {EventType}", webhookEvent.type);
                return;
            }

            var replyMessage = await handler.HandleAsync(webhookEvent, cancellationToken);
            if (replyMessage == null)
            {
                _logger.LogDebug("Handler returned null reply message for event {EventId}", webhookEvent.webhookEventId);
                return;
            }

            await SendReplyMessageAsync(replyMessage, cancellationToken);
        }

        public async Task SendReplyMessageAsync(RequestReplyMessageDto replyMessage, CancellationToken cancellationToken = default)
        {
            if (replyMessage == null)
                throw new ArgumentNullException(nameof(replyMessage));

            if (string.IsNullOrWhiteSpace(replyMessage.replyToken))
            {
                _logger.LogWarning("Reply token is missing or empty");
                return;
            }

            if (replyMessage.messages == null || replyMessage.messages.Count == 0)
            {
                _logger.LogWarning("No messages to send in reply");
                return;
            }

            await SendLineApiRequestAsync(_replyApiUrl, replyMessage, "reply", cancellationToken);
        }

        public async Task SendPushMessageAsync(string userId, List<Message> messages, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId));

            if (messages == null || messages.Count == 0)
                throw new ArgumentException("Messages cannot be null or empty", nameof(messages));

            var pushMessage = new
            {
                to = userId,
                messages = messages
            };

            await SendLineApiRequestAsync(_pushApiUrl, pushMessage, "push", cancellationToken);
        }

        private async Task SendLineApiRequestAsync<T>(string apiUrl, T requestBody, string messageType, CancellationToken cancellationToken)
        {
            using var client = CreateHttpClient();
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, apiUrl)
            {
                Content = JsonContent.Create(requestBody)
            };

            try
            {
                _logger.LogDebug("Sending {MessageType} message to Line API: {ApiUrl}", messageType, apiUrl);

                var response = await client.SendAsync(requestMessage, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogError("Line API request failed with status {StatusCode}. Response: {ErrorContent}", 
                        response.StatusCode, errorContent);
                    response.EnsureSuccessStatusCode();
                }

                _logger.LogInformation("Successfully sent {MessageType} message to Line API", messageType);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed when sending {MessageType} message to Line API", messageType);
                throw;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogWarning(ex, "Request to Line API was cancelled for {MessageType} message", messageType);
                throw;
            }
        }

        private HttpClient CreateHttpClient()
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            client.Timeout = TimeSpan.FromSeconds(30);
            return client;
        }
    }
}
