namespace WeatherBot.Dtos.Webhook
{
    /// <summary>
    /// Webook Request body
    /// https://developers.line.biz/en/reference/messaging-api/#request-body
    /// </summary>
    public class WebhookRequestDto
    {
        public string destination { get; set; }
        public WebhookEventDto[] events { get; set; }
    }
}
