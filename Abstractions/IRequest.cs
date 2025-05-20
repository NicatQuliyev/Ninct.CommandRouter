namespace CommandRouter.Abstractions;

/// <summary>
/// Represents a request that expects a response of type <typeparamref name="TResponse"/>.
/// This is a marker interface used to identify requests handled by <see cref="IRequestHandler{TRequest, TResponse}"/>.
/// </summary>
/// <typeparam name="TResponse">The type of response returned by the request handler.</typeparam>
public interface IRequest<TResponse>;