using System.Reflection;
using CommandRouter.Abstractions;
using CommandRouter.Core;
using Microsoft.Extensions.DependencyInjection;

namespace CommandRouter.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHandlersFromAppDomain(this IServiceCollection services)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .ToArray();
        
        return services.AddHandlers(assemblies);
    }
    public static IServiceCollection AddHandlersFromAssemblies(this IServiceCollection services, params Assembly[] assemblies)
    {
        return services.AddHandlers(assemblies);
    }

    private static IServiceCollection AddHandlers(this IServiceCollection services, Assembly[] assemblies)
    {
        var allTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .ToArray();
        
        var requestHandlers = allTypes
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))
                .Select(i => new { Interface = i, Implementation = t }));

        var behaviours = allTypes
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>))
                .Select(i => new { Interface = i, Implementation = t }));

        var notificationHandlers = allTypes
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(INotificationHandler<>))
                .Select(i => new { Interface = i, Implementation = t }));
        
        foreach (var handler in requestHandlers)
        {
            services.AddTransient(handler.Interface, handler.Implementation);
        }
        
        foreach (var behaviour in behaviours)
        {
            services.AddTransient(behaviour.Interface, behaviour.Implementation);
        }

        foreach (var handler in notificationHandlers)
        {
            services.AddTransient(handler.Interface, handler.Implementation);
        }
        
        services.AddSingleton<CommandPusher>();
        services.AddSingleton<NotificationDispatcher>();

        return services;
    }
}
