namespace CommandRouter.Abstractions;

/// <summary>
/// Defines a pipeline behavior for handling a request of type <typeparamref name="TRequest"/> and returning a response of type <typeparamref name="TResponse"/>.
/// Pipeline behaviors can be used to add cross-cutting concerns such as logging, validation, or transaction handling around request processing.
/// </summary>
/// <typeparam name="TRequest">The type of the request being handled.</typeparam>
/// <typeparam name="TResponse">The type of response returned from handling the request.</typeparam>
public interface IPipelineBehavior<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Handles the request, potentially performing actions before and/or after invoking the next delegate in the pipeline.
    /// </summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="next">The delegate to invoke the next handler or behavior in the pipeline.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation, with a response of type <typeparamref name="TResponse"/>.</returns>
    Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken);
}

/// <summary>
/// Represents a delegate to invoke the next handler or behavior in the request pipeline, returning a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TResponse">The response type.</typeparam>
/// <returns>A task that returns a response of type <typeparamref name="TResponse"/>.</returns>
public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();