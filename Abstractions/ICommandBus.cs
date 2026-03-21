namespace CommandRouter.Abstractions;

/// <summary>
/// Represents a bus that can send commands and publish notifications.
/// Combines the functionality of <see cref="ICommandPusher"/> and <see cref="INotificationDispatcher"/>.
/// </summary>
public interface ICommandBus: ICommandPusher, INotificationDispatcher;