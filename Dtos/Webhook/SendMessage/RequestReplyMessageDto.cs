using System.Text.Json.Serialization;

namespace WeatherBot.Dtos.Webhook.SendMessage
{
    /// <summary>
    /// reply message body
    /// https://developers.line.biz/en/docs/messaging-api/sending-messages/#reply-messages
    /// </summary>
    public class RequestReplyMessageDto
    {
        public string replyToken { get; set; } = string.Empty;
        public List<Message> messages { get; set; } = [];

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? notificationDisabled { get; set; }
    }

    /// <summary>
    /// Base message class supporting multiple Line message types
    /// https://developers.line.biz/en/reference/messaging-api/#message-objects
    /// </summary>
    public class Message
    {
        public string type { get; set; } = string.Empty;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? text { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? packageId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? stickerId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? originalContentUrl { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? previewImageUrl { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? title { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? address { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? latitude { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? longitude { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? altText { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? template { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? contents { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public QuickReply? quickReply { get; set; }

        public static Message Text(string text) => new()
        {
            type = "text",
            text = text
        };

        public static Message Sticker(string packageId, string stickerId) => new()
        {
            type = "sticker",
            packageId = packageId,
            stickerId = stickerId
        };

        public static Message Image(string originalContentUrl, string previewImageUrl) => new()
        {
            type = "image",
            originalContentUrl = originalContentUrl,
            previewImageUrl = previewImageUrl
        };

        public static Message Location(string title, string address, double latitude, double longitude) => new()
        {
            type = "location",
            title = title,
            address = address,
            latitude = latitude,
            longitude = longitude
        };
    }

    public class QuickReply
    {
        public List<QuickReplyItem> items { get; set; } = [];
    }

    public class QuickReplyItem
    {
        public string type { get; set; } = "action";
        public object action { get; set; } = default!;
    }

}

