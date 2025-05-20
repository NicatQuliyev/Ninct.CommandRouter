using CommandRouter.Abstractions;

namespace CommandRouter.Core;

/// <summary>
/// A command bus that acts as a unified entry point for sending commands and publishing notifications.
/// Delegates command sending to an <see cref="ICommandPusher"/> and notification publishing to an <see cref="INotificationDispatcher"/>.
/// Implements both <see cref="ICommandPusher"/> and <see cref="INotificationDispatcher"/> interfaces.
/// </summary>
public class CommandBus(ICommandPusher commandPusher, INotificationDispatcher notificationDispatcher)
    : ICommandBus
{
    /// <inheritdoc />
    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        return commandPusher.Send(request, cancellationToken);
    }

    /// <inheritdoc />
    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
    {
        return notificationDispatcher.Publish(notification, cancellationToken);
    }
}