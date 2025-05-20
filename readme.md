# CommandRouter

**CommandRouter** is a lightweight CQRS-style mediator pattern implementation for .NET projects. It provides a simple way to decouple business logic using commands, queries, and pipeline behaviors — all with automatic handler registration.
> **Note:** This library was developed primarily for my personal use.  
> Anyone is free to copy, modify, and use it in their own projects without restrictions.
---

## ✨ Features

- ✅ `IRequest<TResponse>` and `IRequestHandler<TRequest, TResponse>` for clean request/response flow
- ✅ `IPipelineBehavior<TRequest, TResponse>` support for logging, validation, etc.
- ✅ `INotification` and `INotificationHandler<TNotification>` for pub/sub-style event broadcasting
- ✅ Zero external dependencies
- ✅ Supports automatic handler discovery via assembly scanning

---

## 📦 Installation

Add the project to your solution as a reference or include it via NuGet (coming soon).

```bash
dotnet add package CommandRouter
```

## 🚀 Getting Started
1. Define a Request
```csharp
public class PingRequest : IRequest<string>
{
    public string Message { get; init; }
} 
```
2. Implement a Request Handler
```csharp
public class PingRequestHandler : IRequestHandler<PingRequest, string>
{
    public Task<string> Handle(PingRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult($"Pong: {request.Message}");
    }
}
```
3. Register Services
```csharp
builder.Services.AddHandlersFromAppDomain();
```

4. Send a Request
```csharp
public class MyService
{
    private readonly CommandPusher _pusher;

    public MyService(CommandPusher pusher)
    {
        _pusher = pusher;
    }

    public async Task<string> DoWork()
    {
        var result = await _pusher.Send(new PingRequest { Message = "Hello" });
        return result;
    }
}
```
## 📢 Notifications (Optional)
1. Define a Notification
```csharp
public class UserRegisteredNotification : INotification
{
    public string Email { get; set; }
}
```

2. Handle the Notification
```csharp
public class SendWelcomeEmailHandler : INotificationHandler<UserRegisteredNotification>
{
    public Task Handle(UserRegisteredNotification notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Sending email to {notification.Email}");
        return Task.CompletedTask;
    }
}
```

3. Publish the Notification
```csharp
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        Console.WriteLine($"Handling {typeof(TRequest).Name}");
        var response = await next();
        Console.WriteLine($"Handled {typeof(TResponse).Name}");
        return response;
    }
}
```
## 🧩 Pipeline Behaviors
```csharp
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        Console.WriteLine($"Handling {typeof(TRequest).Name}");
        var response = await next();
        Console.WriteLine($"Handled {typeof(TResponse).Name}");
        return response;
    }
}
```

#### 📄 License
This project is open source and available under the [MIT License.](https://github.com/NicatQuliyev/Ninct.CommandRouter/blob/main/License)