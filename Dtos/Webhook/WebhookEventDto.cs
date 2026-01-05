namespace WeatherBot.Dtos.Webhook
{
    /// <summary>
    /// Message Event Common properties
    /// https://developers.line.biz/en/reference/messaging-api/#common-properties
    /// </summary>
    public class WebhookEventDto
    {
        public string replyToken { get; set; }
        public string type { get; set; }
        public string mode { get; set; }
        public long timestamp { get; set; }
        public Source source { get; set; }
        public string webhookEventId { get; set; }
        public Deliverycontext deliveryContext { get; set; }
        public EventMessageTextDto message { get; set; }
    }

    public class Source
    {
        public string type { get; set; }
        public string groupId { get; set; }
        public string userId { get; set; }
    }

    public class Deliverycontext
    {
        public bool isRedelivery { get; set; }
    }

}
