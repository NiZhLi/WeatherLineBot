namespace WeatherBot.Dtos.Webhook
{
    /// <summary>
    /// Message Text Event
    /// https://developers.line.biz/en/reference/messaging-api/#message-event
    /// </summary>
    public class EventMessageTextDto
    {
        public string id { get; set; }
        public string type { get; set; }
        public string quoteToken { get; set; }
        public string markAsReadToken { get; set; }
        public string text { get; set; }
        public Emoji[] emojis { get; set; }
        public Mention mention { get; set; }
    }

    public class Mention
    {
        public Mentionee[] mentionees { get; set; }
    }

    public class Mentionee
    {
        public int index { get; set; }
        public int length { get; set; }
        public string type { get; set; }
        public string userId { get; set; }
        public bool isSelf { get; set; }
    }

    public class Emoji
    {
        public int index { get; set; }
        public int length { get; set; }
        public string productId { get; set; }
        public string emojiId { get; set; }
    }
}
