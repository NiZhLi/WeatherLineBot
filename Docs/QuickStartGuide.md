# Line Bot Quick Start Guide

## Common Tasks

### 1. Creating a Text Reply

```csharp
// Simple way
var reply = LineMessageBuilder.CreateReplyMessage(
    replyToken,
    LineMessageBuilder.CreateTextMessage("Hello, World!")
);

await _lineBotService.SendReplyMessageAsync(reply);
```

### 2. Creating Multiple Message Reply

```csharp
var reply = new RequestReplyMessageDto
{
    replyToken = replyToken,
    messages = new List<Message>
    {
        LineMessageBuilder.CreateTextMessage("First message"),
        LineMessageBuilder.CreateTextMessage("Second message"),
        LineMessageBuilder.CreateStickerMessage("1", "1")
    }
};

await _lineBotService.SendReplyMessageAsync(reply);
```

### 3. Sending Push Message (Proactive Message)

```csharp
var messages = new List<Message>
{
    LineMessageBuilder.CreateTextMessage("This is a push notification!"),
    LineMessageBuilder.CreateImageMessage(
        "https://example.com/image.jpg",
        "https://example.com/preview.jpg"
    )
};

await _lineBotService.SendPushMessageAsync(userId, messages);
```

### 4. Creating a New Event Handler

```csharp
using Microsoft.Extensions.Logging;
using WeatherBot.Dtos.Webhook;
using WeatherBot.Dtos.Webhook.SendMessage;
using WeatherBot.Services.LineMessaging;

namespace WeatherBot.Services.LineMessaging.Handlers
{
    public class MyCustomEventHandler : BaseWebhookEventHandler<MyCustomEventHandler>
    {
        private readonly IMyService _myService;

        public MyCustomEventHandler(
            IMyService myService,
            ILogger<MyCustomEventHandler> logger) 
            : base(logger)
        {
            _myService = myService;
        }

        public override bool CanHandle(WebhookEventDto webhookEvent)
        {
            return webhookEvent.type == "beacon"; // Example: beacon event
        }

        protected override async Task<RequestReplyMessageDto?> HandleEventAsync(
            WebhookEventDto webhookEvent, 
            CancellationToken cancellationToken)
        {
            var userId = GetUserId(webhookEvent);
            Logger.LogInformation("Beacon detected for user {UserId}", userId);

            var responseText = await _myService.ProcessBeacon(webhookEvent, cancellationToken);
            
            return CreateTextReplyMessage(webhookEvent.replyToken, responseText);
        }
    }
}
```

**Register in Program.cs:**
```csharp
builder.Services.AddScoped<IWebhookEventHandler, MyCustomEventHandler>();
```

### 5. Creating a New Message Strategy

```csharp
using WeatherBot.Dtos.Webhook;
using WeatherBot.Services.LineMessaging.Strategies;

namespace WeatherBot.Services.LineMessaging.Strategies
{
    public class AudioMessageStrategy : IMessageStrategy
    {
        private readonly ILogger<AudioMessageStrategy> _logger;

        public AudioMessageStrategy(ILogger<AudioMessageStrategy> logger)
        {
            _logger = logger;
        }

        public bool CanHandle(WebhookEventDto webhookEvent)
        {
            return string.Equals(
                webhookEvent.message?.type, 
                "audio", 
                StringComparison.OrdinalIgnoreCase
            );
        }

        public async Task<string?> CreateReplyAsync(
            WebhookEventDto webhookEvent, 
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Received audio message from {UserId}", 
                webhookEvent.source?.userId);

            // Process audio message
            return "Thanks for the audio message! 🎵";
        }
    }
}
```

**Register in Program.cs:**
```csharp
builder.Services.AddScoped<IMessageStrategy, AudioMessageStrategy>();
```

### 6. Handling Different Message Types

```csharp
public class MultiMediaStrategy : IMessageStrategy
{
    public bool CanHandle(WebhookEventDto webhookEvent)
    {
        var messageType = webhookEvent.message?.type?.ToLower();
        return messageType is "image" or "video" or "audio";
    }

    public async Task<string?> CreateReplyAsync(
        WebhookEventDto webhookEvent, 
        CancellationToken cancellationToken)
    {
        return webhookEvent.message?.type?.ToLower() switch
        {
            "image" => "Nice picture! 📷",
            "video" => "Great video! 🎬",
            "audio" => "Love the audio! 🎵",
            _ => "Thanks for sharing!"
        };
    }
}
```

### 7. Using Services in Event Handlers

```csharp
public class WeatherFollowHandler : BaseWebhookEventHandler<WeatherFollowHandler>
{
    private readonly DomainWeatherService _weatherService;

    public WeatherFollowHandler(
        DomainWeatherService weatherService,
        ILogger<WeatherFollowHandler> logger) 
        : base(logger)
    {
        _weatherService = weatherService;
    }

    public override bool CanHandle(WebhookEventDto webhookEvent)
    {
        return webhookEvent.type == "follow";
    }

    protected override async Task<RequestReplyMessageDto?> HandleEventAsync(
        WebhookEventDto webhookEvent, 
        CancellationToken cancellationToken)
    {
        // Get weather for default city
        var weather = await _weatherService.GetTodayWeatherInfoAsync(
            DateTime.Now, 
            "臺北市"
        );

        var welcomeMessage = $"歡迎使用天氣機器人！\n\n今日臺北天氣：\n{weather}";

        return CreateTextReplyMessage(webhookEvent.replyToken, welcomeMessage);
    }
}
```

### 8. Custom Error Handling

```csharp
public class SafeEventHandler : BaseWebhookEventHandler<SafeEventHandler>
{
    public SafeEventHandler(ILogger<SafeEventHandler> logger) : base(logger) { }

    public override bool CanHandle(WebhookEventDto webhookEvent)
    {
        return webhookEvent.type == "message";
    }

    protected override async Task<RequestReplyMessageDto?> HandleEventAsync(
        WebhookEventDto webhookEvent, 
        CancellationToken cancellationToken)
    {
        try
        {
            // Your risky operation
            var result = await RiskyOperation();
            return CreateTextReplyMessage(webhookEvent.replyToken, result);
        }
        catch (CustomException ex)
        {
            Logger.LogWarning(ex, "Custom exception occurred");
            return CreateTextReplyMessage(
                webhookEvent.replyToken, 
                "抱歉，暫時無法處理您的請求。"
            );
        }
    }

    // Override default error message
    protected override RequestReplyMessageDto? CreateErrorReplyMessage(string replyToken)
    {
        return CreateTextReplyMessage(
            replyToken,
            "系統發生錯誤，我們的團隊正在處理中。請稍後再試。"
        );
    }
}
```

### 9. Working with Quick Replies

```csharp
public class QuickReplyExample
{
    public RequestReplyMessageDto CreateQuickReplyMessage(string replyToken)
    {
        return new RequestReplyMessageDto
        {
            replyToken = replyToken,
            messages = new List<Message>
            {
                new Message
                {
                    type = "text",
                    text = "請選擇一個選項：",
                    quickReply = new QuickReply
                    {
                        items = new List<QuickReplyItem>
                        {
                            new QuickReplyItem
                            {
                                type = "action",
                                action = new
                                {
                                    type = "message",
                                    label = "今天天氣",
                                    text = "今天天氣"
                                }
                            },
                            new QuickReplyItem
                            {
                                type = "action",
                                action = new
                                {
                                    type = "message",
                                    label = "明天天氣",
                                    text = "明天天氣"
                                }
                            }
                        }
                    }
                }
            }
        };
    }
}
```

### 10. Conditional Replies Based on User ID

```csharp
public class PersonalizedHandler : BaseWebhookEventHandler<PersonalizedHandler>
{
    private readonly IUserService _userService;

    public PersonalizedHandler(
        IUserService userService,
        ILogger<PersonalizedHandler> logger) 
        : base(logger)
    {
        _userService = userService;
    }

    protected override async Task<RequestReplyMessageDto?> HandleEventAsync(
        WebhookEventDto webhookEvent, 
        CancellationToken cancellationToken)
    {
        var userId = GetUserId(webhookEvent);
        var user = await _userService.GetUserAsync(userId);

        var greeting = user?.IsVip == true
            ? $"歡迎回來，尊貴的 {user.Name}！"
            : "您好！";

        return CreateTextReplyMessage(webhookEvent.replyToken, greeting);
    }
}
```

## Testing Tips

### Unit Testing Event Handlers

```csharp
[Fact]
public async Task HandleEventAsync_ShouldReturnWelcomeMessage()
{
    // Arrange
    var logger = new Mock<ILogger<FollowWebhookEventHandler>>();
    var handler = new FollowWebhookEventHandler(logger.Object);
    
    var webhookEvent = new WebhookEventDto
    {
        type = "follow",
        replyToken = "test-token",
        source = new Source { userId = "test-user" }
    };

    // Act
    var result = await handler.HandleAsync(webhookEvent, CancellationToken.None);

    // Assert
    Assert.NotNull(result);
    Assert.Equal("test-token", result.replyToken);
    Assert.Contains("歡迎", result.messages[0].text);
}
```

### Integration Testing

```csharp
[Fact]
public async Task WebhookController_ShouldProcessWebhook()
{
    // Arrange
    var lineBotService = new Mock<ILineBotService>();
    var logger = new Mock<ILogger<WebhookController>>();
    var controller = new WebhookController(lineBotService.Object, logger.Object);

    var request = new WebhookRequestDto
    {
        events = new[]
        {
            new WebhookEventDto { type = "message", replyToken = "token" }
        }
    };

    // Act
    var result = await controller.CreateWebhook(request);

    // Assert
    Assert.IsType<OkResult>(result);
    lineBotService.Verify(s => s.HandleWebhookAsync(request, default), Times.Once);
}
```

## Configuration

Ensure your `appsettings.json` contains:

```json
{
  "LineWebhook": {
    "ChannelAccessToken": "your-channel-access-token",
    "ChannelSecret": "your-channel-secret"
  }
}
```

## Common Issues

### Issue: Handler not being called
**Solution**: Ensure handler is registered in `Program.cs` and `CanHandle()` returns true

### Issue: Message not sent
**Solution**: Check logs for HTTP errors, verify access token is correct

### Issue: Multiple handlers responding
**Solution**: Make sure `CanHandle()` methods are mutually exclusive

## Best Practices

1. ✅ Always use `LineMessageBuilder` for creating messages
2. ✅ Extend `BaseWebhookEventHandler` for new handlers
3. ✅ Use dependency injection for all services
4. ✅ Log important events and errors
5. ✅ Handle exceptions gracefully
6. ✅ Use cancellation tokens for async operations
7. ✅ Keep handlers focused on a single responsibility
8. ✅ Test handlers independently

## Resources

- [LINE Messaging API Documentation](https://developers.line.biz/en/reference/messaging-api/)
- [Webhook Events Reference](https://developers.line.biz/en/reference/messaging-api/#webhook-event-objects)
- [Message Types Reference](https://developers.line.biz/en/reference/messaging-api/#message-objects)
