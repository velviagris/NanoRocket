using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NanoRocket.DependencyInjection;

public static class RocketConsumerServiceExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="key"></param>
    /// <typeparam name="TMessageHandler"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddRocketConsumerService<TMessageHandler>(this IServiceCollection services,
        string key) where TMessageHandler : class, IMessageHandler
    {
        services.AddKeyedSingleton<IMessageHandler, TMessageHandler>(key);
        services.AddSingleton<IHostedService>(x =>
        {
            var logger = x.GetRequiredService<ILogger<RocketConsumerService>>();
            var config = x.GetRequiredService<IConfiguration>();
            var service = new RocketConsumerService(logger, config, x, key,
                key);
            return service;
        });

        return services;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="key"></param>
    /// <typeparam name="TMessageHandler"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddRocketConsumerServiceFromAppSettings<TMessageHandler>(this IServiceCollection services,
        string key) where TMessageHandler : class, IMessageHandler
    {
        services.AddKeyedSingleton<IMessageHandler, TMessageHandler>(key);
        services.AddSingleton<IHostedService>(x =>
        {
            var logger = x.GetRequiredService<ILogger<RocketConsumerService>>();
            var config = x.GetRequiredService<IConfiguration>();
            var service = new RocketConsumerService(logger, config, x, key,
                key);
            return service;
        });

        return services;
    }
}