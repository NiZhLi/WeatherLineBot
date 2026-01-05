namespace WeatherBot.Dtos.Webhook.SendMessage
{
    /// <summary>
    /// reply message body
    /// https://developers.line.biz/en/docs/messaging-api/sending-messages/#reply-messages
    /// </summary>
    public class RequestReplyMessageDto
    {
        public string replyToken { get; set; }
        public List<Message> messages { get; set; }
    }

    public class Message
    {
        public string type { get; set; }
        public string text { get; set; }
    }

}
