using System.Reflection;
using CommandRouter.Abstractions;
using CommandRouter.Core;
using Microsoft.Extensions.DependencyInjection;

namespace CommandRouter.Extensions;

/// <summary>
/// Extension methods to register CQRS handlers, behaviors, and notification handlers into the DI container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Scans all loaded assemblies in the current app domain and registers all request handlers,
    /// pipeline behaviors, and notification handlers found.
    /// </summary>
    /// <param name="services">The service collection to add handlers to.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddHandlersFromAppDomain(this IServiceCollection services)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .ToArray();
        
        return services.AddHandlers(assemblies);
    }
    
    /// <summary>
    /// Registers all request handlers, pipeline behaviors, and notification handlers found in the specified assemblies.
    /// </summary>
    /// <param name="services">The service collection to add handlers to.</param>
    /// <param name="assemblies">The assemblies to scan for handlers.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddHandlersFromAssemblies(this IServiceCollection services, params Assembly[] assemblies)
    {
        return services.AddHandlers(assemblies);
    }

    /// <summary>
    /// Registers all request handlers, pipeline behaviors, and notification handlers found in the specified assemblies.
    /// Also registers the <see cref="ICommandPusher"/>, <see cref="INotificationDispatcher"/> and <see cref="ICommandBus"/>> services.
    /// </summary>
    /// <param name="services">The service collection to add handlers to.</param>
    /// <param name="assemblies">The assemblies to scan for handlers.</param>
    /// <returns>The updated service collection.</returns>
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
        
        services.AddTransient<ICommandPusher, CommandPusher>();
        services.AddTransient<INotificationDispatcher, NotificationDispatcher>();
        services.AddTransient<CommandBus>();

        return services;
    }
}
