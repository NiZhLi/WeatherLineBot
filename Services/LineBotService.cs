using Microsoft.AspNetCore.Authentication.BearerToken;
using WeatherBot.Dtos.Webhook;
using WeatherBot.Dtos.Webhook.SendMessage;
using System.Net.Http.Headers;

namespace WeatherBot.Services
{
    public class LineBotService(IConfiguration configuration, IHttpClientFactory httpClient)
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

            var replyText = webhookEvent.message.text;
            var replyMessage = new RequestReplyMessageDto()
            {
                replyToken = webhookEvent.replyToken,
                messages = new List<Message>
                {
                    new Message
                    {
                        type = "text",
                        text = $"你傳送的地址是：{replyText}"
                    }
                }

            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, replyApiUrl)
            {
                Content = JsonContent.Create(replyMessage)
            };

            var response = await client.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();
        }
        //public RequestReplyMessageDto gernate()
        //{
        //    var textFormat = 
        //        {
        //        "今天天氣":"36.c"
        //    }
        //    return new RequestReplyMessageDto();
        //}
    }
}
