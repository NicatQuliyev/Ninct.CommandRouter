using CommandRouter.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace CommandRouter.Core;

/// <summary>
/// Responsible for sending requests to their respective handlers, applying all registered pipeline behaviors.
/// Implements the mediator pattern by resolving the appropriate IRequestHandler and IPipelineBehavior instances
/// from the DI container, then executing them in order.
/// </summary>
/// <param name="serviceProvider">The service provider used to resolve handlers and pipeline behaviors.</param>
public class CommandPusher(IServiceProvider serviceProvider)
{
    /// <summary>
    /// Sends a request through the pipeline behaviors to the corresponding handler and returns the handler's response.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response expected from the request handler.</typeparam>
    /// <param name="request">The request object implementing IRequest with the expected response type.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the request processing.</param>
    /// <returns>A Task that represents the asynchronous operation, containing the response from the request handler.</returns>
    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));
        dynamic handler = serviceProvider.GetRequiredService(handlerType);

        var behaviors = serviceProvider
            .GetServices(typeof(IPipelineBehavior<,>)
                .MakeGenericType(requestType, typeof(TResponse)))
            .Cast<dynamic>()
            .Reverse()
            .ToList();

        RequestHandlerDelegate<TResponse> handlerDelegate = () => handler.Handle((dynamic)request, cancellationToken);

        foreach (var behavior in behaviors)
        {
            var next = handlerDelegate;
            handlerDelegate = () => behavior.Handle((dynamic)request, next, cancellationToken);
        }

        return await handlerDelegate();
    }
}