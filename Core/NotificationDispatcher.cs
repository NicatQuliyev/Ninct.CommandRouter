using CommandRouter.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace CommandRouter.Core;

/// <summary>
/// Responsible for publishing notifications to all registered notification handlers.
/// </summary>
/// <param name="serviceProvider">The service provider used to resolve notification handlers.</param>
public class NotificationDispatcher(IServiceProvider serviceProvider)
{
    /// <summary>
    /// Publishes a notification to all registered <see cref="INotificationHandler{TNotification}"/> instances.
    /// </summary>
    /// <typeparam name="TNotification">The type of notification being published.</typeparam>
    /// <param name="notification">The notification instance to publish.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the notification publishing.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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