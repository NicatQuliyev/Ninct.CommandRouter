namespace CommandRouter.Abstractions;

/// <summary>
/// Defines a contract for publishing notifications to registered notification handlers.
/// </summary>
public interface INotificationDispatcher
{
    /// <summary>
    /// Publishes a notification to all registered <see cref="INotificationHandler{TNotification}"/> instances.
    /// </summary>
    /// <typeparam name="TNotification">The type of notification being published.</typeparam>
    /// <param name="notification">The notification instance to publish.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the notification publishing.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification;
}