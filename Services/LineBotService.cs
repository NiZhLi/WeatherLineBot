using Microsoft.AspNetCore.Authentication.BearerToken;
using WeatherBot.Dtos.Webhook;
using WeatherBot.Dtos.Webhook.SendMessage;
using System.Net.Http.Headers;

namespace WeatherBot.Services
{
    public class LineBotService(IConfiguration configuration, IHttpClientFactory httpClient, DomainWeatherService dWeatherService, ILogger<LineBotService> logger)
    {
        private readonly string accessToken = configuration["LineWebhook:ChannelAccessToken"];
        private readonly string secret = configuration["LineWebhook:ChannelSecret"];
        private readonly string replyApiUrl = "https://api.line.me/v2/bot/message/reply";

        public async Task HandleWebhookAsync(WebhookRequestDto WebhookRequestDto)
        {
            foreach (var webhookEvent in WebhookRequestDto.events)
            {
                switch (webhookEvent.type)
                {
                    case "message":
                        
                        await ReplyMessageAsync(webhookEvent);
                        break;
                    default:
                        Console.WriteLine("webhook events got wrong");
                        break;
                }
            }
            return;
        }

        public async Task ReplyMessageAsync(WebhookEventDto webhookEvent)
        {
            var client = httpClient.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var userText = webhookEvent.message.text;
            var replyText = string.Empty;
            if (!IsMessageTextLocation(userText))
            {
                replyText = "請加上縣市";
            }

            // TODO: use utc time
            replyText = await dWeatherService.GetTomorrowWeatherInfoAsync(DateTime.Now, userText);

            var replyMessage = new RequestReplyMessageDto()
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

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, replyApiUrl)
            {
                Content = JsonContent.Create(replyMessage)
            };


            var response = await client.SendAsync(requestMessage);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex) 
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                logger.LogError(ex, "HTTP request failed with content: {ErrorContent}", errorContent);
            }
        }

        public bool IsMessageTextLocation(string messageText)
        {


            var validLocations = new List<string> 
            {
                "臺北市", "新北市", "桃園市", "臺中市", "臺南市", "高雄市",
                "基隆市", "新竹市", "嘉義市", "新竹縣", "苗栗縣", "彰化縣",
                "南投縣", "雲林縣", "嘉義縣", "屏東縣", "宜蘭縣", "花蓮縣",
                "臺東縣", "澎湖縣", "金門縣", "連江縣"
            };
            return validLocations.Contains(messageText);
        }
    }
}
