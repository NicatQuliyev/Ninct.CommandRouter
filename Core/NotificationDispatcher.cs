using CommandRouter.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace CommandRouter.Core;

public class NotificationDispatcher(IServiceProvider serviceProvider)
{
    public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        var handlers = serviceProvider.GetServices<INotificationHandler<TNotification>>();

        foreach (var handler in handlers)
        {
            await handler.Handle(notification, cancellationToken);
        }
    }
}