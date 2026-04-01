using System.Text.Json.Serialization;

namespace WeatherBot.Dtos.Webhook
{
    /// <summary>
    /// Message Event Common properties
    /// https://developers.line.biz/en/reference/messaging-api/#common-properties
    /// </summary>
    public class WebhookEventDto
    {
        public string replyToken { get; set; } = string.Empty;
        public string type { get; set; } = string.Empty;
        public string mode { get; set; } = string.Empty;
        public long timestamp { get; set; }
        public Source source { get; set; } = new();
        public string webhookEventId { get; set; } = string.Empty;
        public DeliveryContextDto deliveryContext { get; set; } = new();
        public EventMessageTextDto? message { get; set; }
        public PostbackDto? postback { get; set; }
    }

    public class Source
    {
        public string type { get; set; } = string.Empty;
        public string? groupId { get; set; }
        public string? userId { get; set; }
    }

    public class DeliveryContextDto
    {
        public bool isRedelivery { get; set; }
    }

    public class PostbackDto
    {
        public string? data { get; set; }

        [JsonPropertyName("params")]
        public object? paramsData { get; set; }
    }

}
