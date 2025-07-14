using Example.ReadSettingConsumer;
using Microsoft.Extensions.Hosting;
using NanoRocket.DependencyInjection;
using NLog;


var logger = LogManager.GetCurrentClassLogger();
logger.Debug("init main");

try
{
    var app = CreateHostBuilder(args).Build();
    await app.RunAsync();
}
catch (Exception ex)
{
    logger.Error(ex, ex.Message);
    throw;
}
finally
{
    LogManager.Shutdown();
}

IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.AddRocketConsumerServiceFromAppSettings<FooTopicHandler>("FooTopicHandlerConfig");
        });