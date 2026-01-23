using Microsoft.AspNetCore.Authentication.BearerToken;
using System.Linq;
using System.Net.Http.Headers;
using WeatherBot.Dtos.Webhook;
using WeatherBot.Dtos.Webhook.SendMessage;

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
            var userTextType = webhookEvent.message.type;
            var replyText = "( ^ω^) 看不懂喔";

            // 判斷傳遞的為位置資訊或文字
            if (userTextType == "text")
            {
                userText = GetLocationFromMessage(userText);

                // TODO: use utc time
                replyText = await dWeatherService.GetTomorrowWeatherInfoAsync(DateTime.Now, userText);
            }
            else if (userTextType == "location")
            {
                // TODO: use utc time
                // 因直接獲取line json text作為地址參數查詢，故使用者傳送GPS資訊時可以直接存取
                replyText = await dWeatherService.GetTomorrowWeatherInfoAsync(DateTime.Now, userText);
            }


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

        private async Task SaveLocationToRedisAsync(string userId, object locationData)
        {
            // 假設已經有 Redis 客戶端實例化邏輯
            // 使用 Redis 儲存邏輯 (需根據實際 Redis 套件實作)
            // 例如：await redisClient.SetAsync($"user:location:{userId}", locationData);
                throw new NotImplementedException("請實作 Redis 儲存邏輯");
        }

        public string GetLocationFromMessage(string messageText)
        {
            var validLocations = new HashSet<string>
            {
                "臺北", "新北", "桃園", "臺中", "臺南", "高雄",
                "基隆", "新竹", "嘉義", "苗栗", "彰化", "南投",
                "雲林", "屏東", "宜蘭", "花蓮", "臺東", "澎湖",
                "金門", "連江"
            };

            var citys = new HashSet<string>
            {
                "臺北", "新北", "桃園", "臺中", "臺南", "高雄", "基隆"
            };

            var countrys = new HashSet<string>
            {
                "苗栗", "彰化", "南投", "雲林", "屏東", "宜蘭", "花蓮", "臺東", "澎湖", "金門", "連江"
            };

            var bothCityAndCounty = new HashSet<string>
            {
                "新竹", "嘉義"
            };

            if (string.IsNullOrWhiteSpace(messageText) || messageText.Length > 1000)
            {
                return messageText;
            }

            // 將台字替換為臺，進行模糊匹配
            var normalizedMessage = messageText.Replace("台", "臺");

            // 檢查訊息中是否包含有效的臺灣縣市名稱
            foreach (var location in validLocations)
            {
                if (normalizedMessage.Contains(location))
                {
                    // 如果是市
                    if (citys.Contains(location))
                    {
                        return location + "市";
                    }
                    // 如果是縣
                    else if (countrys.Contains(location))
                    {
                        return location + "縣";
                    }
                    // 如果為新竹或嘉義(縣市皆有)，預設回傳市，未來可擴充其他判斷邏輯
                    else if (bothCityAndCounty.Contains(location))
                    {
                        return location + "市";
                    }
                }
            }

            // 如果不包含有效縣市名稱，回傳原始訊息
            return messageText;
        }
    }
}
