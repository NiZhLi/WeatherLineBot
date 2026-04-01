using System.Text.Json.Serialization;

namespace WeatherBot.Dtos.Webhook
{
    /// <summary>
    /// Message Text Event
    /// https://developers.line.biz/en/reference/messaging-api/#message-event
    /// </summary>
    public class EventMessageTextDto
    {
        public string id { get; set; } = string.Empty;
        public string type { get; set; } = string.Empty;
        public string? quoteToken { get; set; }
        public string? markAsReadToken { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? text { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? title { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? address { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? latitude { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? longitude { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? packageId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? stickerId { get; set; }

        public Emoji[] emojis { get; set; } = [];

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Mention? mention { get; set; }
    }

    public class Mention
    {
        public Mentionee[] mentionees { get; set; } = [];
    }

    public class Mentionee
    {
        public int index { get; set; }
        public int length { get; set; }
        public string type { get; set; } = string.Empty;
        public string? userId { get; set; }
        public bool isSelf { get; set; }
    }

    public class Emoji
    {
        public int index { get; set; }
        public int length { get; set; }
        public string productId { get; set; } = string.Empty;
        public string emojiId { get; set; } = string.Empty;
    }
}
