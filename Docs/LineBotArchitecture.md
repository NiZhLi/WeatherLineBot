# Line Bot Service Architecture

## Overview
This Line Bot service uses a modular, extensible architecture based on the Strategy and Chain of Responsibility patterns for handling LINE Messaging API webhook events.

## Architecture Components

### 1. Service Layer
**`ILineBotService` / `LineBotService`**
- Core service for handling LINE webhook events and sending messages
- Supports reply messages and push messages
- Provides centralized HTTP client management and error handling
- Parallel processing of multiple webhook events

### 2. Webhook Event Handlers
**Pattern**: Chain of Responsibility
**Base Class**: `BaseWebhookEventHandler<TLogger>`

Each handler implements `IWebhookEventHandler` and processes specific event types:

- **`MessageWebhookEventHandler`**: Handles incoming messages (delegates to message strategies)
- **`FollowWebhookEventHandler`**: Handles user follow events
- **`UnfollowWebhookEventHandler`**: Handles user unfollow events
- **`PostbackWebhookEventHandler`**: Handles postback events from interactive components

#### Creating a New Event Handler
```csharp
public class MyCustomEventHandler : BaseWebhookEventHandler<MyCustomEventHandler>
{
    public MyCustomEventHandler(ILogger<MyCustomEventHandler> logger) 
        : base(logger) { }

    public override bool CanHandle(WebhookEventDto webhookEvent)
    {
        return webhookEvent.type == "customEventType";
    }

    protected override Task<RequestReplyMessageDto?> HandleEventAsync(
        WebhookEventDto webhookEvent, 
        CancellationToken cancellationToken)
    {
        // Implement your logic here
        return Task.FromResult<RequestReplyMessageDto?>(
            CreateTextReplyMessage(webhookEvent.replyToken, "Custom response")
        );
    }
}
```

Register in `Program.cs`:
```csharp
builder.Services.AddScoped<IWebhookEventHandler, MyCustomEventHandler>();
```

### 3. Message Strategies
**Pattern**: Strategy Pattern
**Interface**: `IMessageStrategy`

Message strategies handle different types of incoming messages:

- **`KeywordReplyMessageStrategy`**: Handles keyword-based replies
- **`LocationMessageStrategy`**: Handles location messages
- **`CityWeatherMessageStrategy`**: Handles city weather queries

#### Creating a New Message Strategy
```csharp
public class MyMessageStrategy : IMessageStrategy
{
    public bool CanHandle(WebhookEventDto webhookEvent)
    {
        return webhookEvent.message?.type == "myMessageType";
    }

    public async Task<string?> CreateReplyAsync(
        WebhookEventDto webhookEvent, 
        CancellationToken cancellationToken)
    {
        // Process the message and return reply text
        return "Reply text";
    }
}
```

Register in `Program.cs`:
```csharp
builder.Services.AddScoped<IMessageStrategy, MyMessageStrategy>();
```

### 4. Message Builder
**`LineMessageBuilder`**
Helper class for constructing LINE message objects:

```csharp
// Text message
var textMsg = LineMessageBuilder.CreateTextMessage("Hello!");

// Sticker message
var stickerMsg = LineMessageBuilder.CreateStickerMessage("1", "1");

// Image message
var imageMsg = LineMessageBuilder.CreateImageMessage(
    "https://example.com/image.jpg",
    "https://example.com/preview.jpg"
);

// Location message
var locationMsg = LineMessageBuilder.CreateLocationMessage(
    "My Location", 
    "123 Street", 
    35.6762, 
    139.6503
);

// Reply with multiple messages
var reply = LineMessageBuilder.CreateReplyMessage(
    replyToken,
    textMsg,
    stickerMsg
);
```

## Data Flow

1. **Webhook Request** → `WebhookController.CreateWebhook()`
2. **Controller** → `ILineBotService.HandleWebhookAsync()`
3. **Service** → Iterates through events, finds matching `IWebhookEventHandler`
4. **Event Handler** → Processes event (may delegate to message strategies)
5. **Handler** → Returns `RequestReplyMessageDto`
6. **Service** → Sends reply to LINE API

## Extension Points

### Adding New Event Types
1. Create a new handler class extending `BaseWebhookEventHandler<T>`
2. Implement `CanHandle()` and `HandleEventAsync()`
3. Register in DI container

### Adding New Message Types
1. Create a new strategy class implementing `IMessageStrategy`
2. Implement `CanHandle()` and `CreateReplyAsync()`
3. Register in DI container

### Sending Push Messages
```csharp
public class MyService
{
    private readonly ILineBotService _lineBotService;

    public async Task SendNotification(string userId)
    {
        var messages = new List<Message>
        {
            LineMessageBuilder.CreateTextMessage("Notification!")
        };
        
        await _lineBotService.SendPushMessageAsync(userId, messages);
    }
}
```

## Benefits

✅ **Extensibility**: Easy to add new event handlers and message strategies
✅ **Maintainability**: Separation of concerns with clear responsibilities
✅ **Testability**: Interface-based design supports unit testing
✅ **Reusability**: Base classes and helper methods reduce code duplication
✅ **Scalability**: Parallel event processing for better performance
✅ **Type Safety**: Strong typing with DTOs matching LINE API

## LINE API Reference
- [Messaging API Reference](https://developers.line.biz/en/reference/messaging-api/)
- [Webhook Events](https://developers.line.biz/en/reference/messaging-api/#webhook-event-objects)
- [Message Objects](https://developers.line.biz/en/reference/messaging-api/#message-objects)
