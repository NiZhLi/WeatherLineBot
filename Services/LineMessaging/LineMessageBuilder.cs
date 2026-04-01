using System.Collections.Generic;
using WeatherBot.Dtos.Webhook.SendMessage;

namespace WeatherBot.Services.LineMessaging
{
    /// <summary>
    /// Fluent builder for constructing Line message objects
    /// https://developers.line.biz/en/reference/messaging-api/#message-objects
    /// </summary>
    public class LineMessageBuilder
    {
        public static Message CreateTextMessage(string text)
        {
            return new Message
            {
                type = "text",
                text = text
            };
        }

        public static Message CreateStickerMessage(string packageId, string stickerId)
        {
            return new Message
            {
                type = "sticker",
                packageId = packageId,
                stickerId = stickerId
            };
        }

        public static Message CreateImageMessage(string originalContentUrl, string previewImageUrl)
        {
            return new Message
            {
                type = "image",
                originalContentUrl = originalContentUrl,
                previewImageUrl = previewImageUrl
            };
        }

        public static Message CreateLocationMessage(string title, string address, double latitude, double longitude)
        {
            return new Message
            {
                type = "location",
                title = title,
                address = address,
                latitude = latitude,
                longitude = longitude
            };
        }

        public static RequestReplyMessageDto CreateReplyMessage(string replyToken, params Message[] messages)
        {
            return new RequestReplyMessageDto
            {
                replyToken = replyToken,
                messages = new List<Message>(messages)
            };
        }

        public static RequestReplyMessageDto CreateReplyMessage(string replyToken, List<Message> messages)
        {
            return new RequestReplyMessageDto
            {
                replyToken = replyToken,
                messages = messages
            };
        }
    }
}
