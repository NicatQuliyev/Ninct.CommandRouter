using CommandRouter.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace CommandRouter.Core;

public class CommandPusher(IServiceProvider serviceProvider)
{
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