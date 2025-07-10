using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Org.Apache.Rocketmq;

namespace NanoRocket;

public class RocketConsumerService : IHostedService,  IDisposable
{
    private readonly ILogger<RocketConsumerService> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _configKey;
    private readonly string _handlerKey;
    private SimpleConsumer _simpleConsumer;
    private readonly IServiceProvider _serviceProvider;
    private CancellationTokenSource _consumerCts;

    public RocketConsumerService(
        ILogger<RocketConsumerService> logger,
        IConfiguration configuration, IServiceProvider serviceProvider, string configKey, string handlerKey)
    {
        _logger = logger;
        _configuration = configuration;
        _configKey = configKey;
        _serviceProvider = serviceProvider;
        _handlerKey = handlerKey;
        _consumerCts = new CancellationTokenSource();
    }


    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.Register(() => _consumerCts.Cancel());
            
            var config = _configuration.GetSection(_configKey);
            var consumerGroup = config.GetValue<string>("ConsumerGroup") ??
                                throw new Exception("ConsumerGroup should not be null");
            var topic = config.GetValue<string>("Topic") ?? throw new Exception("Topic should not be null");
            var endpoints = config.GetValue<string>("Endpoints") ?? throw new Exception("Endpoints should not be null");
            var accessKey = config.GetValue<string>("AccessKey");
            var secretKey = config.GetValue<string>("SecretKey");

            var messageHandler = _serviceProvider.GetKeyedService<IMessageHandler>(_handlerKey) ??
                                 throw new Exception($"Could not find handler by handlerKey: {_handlerKey}");

            _logger.LogInformation($"ConsumerGroup [{consumerGroup}] is connecting RocketMQ, Topic: [{topic}]...");

            // Enable the switch if you use .NET Core 3.1 and want to disable TLS/SSL.
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);


            // Add your subscriptions.
            var subscription = new Dictionary<string, FilterExpression>
                { { topic, new FilterExpression("*") } };
            // In most case, you don't need to create too many consumers, single pattern is recommended.
            var simpleConsumerBuilder = new SimpleConsumer.Builder()
                .SetConsumerGroup(consumerGroup)
                .SetAwaitDuration(TimeSpan.FromSeconds(15))
                .SetSubscriptionExpression(subscription);
            // Credential provider is optional for client configuration.
            var clientConfigBuilder = new ClientConfig.Builder()
                .SetEndpoints(endpoints)
                .EnableSsl(false);
            if (!string.IsNullOrEmpty(accessKey) && !string.IsNullOrEmpty(secretKey))
            {
                var credentialsProvider = new StaticSessionCredentialsProvider(accessKey, secretKey);
                clientConfigBuilder
                    .SetCredentialsProvider(credentialsProvider);
            }

            var clientConfig = clientConfigBuilder.Build();
            simpleConsumerBuilder.SetClientConfig(clientConfig);

            _simpleConsumer = await simpleConsumerBuilder.Build();
            
            _logger.LogInformation($"{endpoints}|{topic}|Consumer initialized");
            
            _ = Task.Run(async () =>
            {
                while (!_consumerCts.Token.IsCancellationRequested)
                {
                    try
                    {
                        // Receive messages
                        var messageViews = await _simpleConsumer.Receive(16, TimeSpan.FromSeconds(15));
                        if (messageViews != null && messageViews.Any())
                        {
                            foreach (var message in messageViews)
                            {
                                await messageHandler.HandleMessage(message);
                                _logger.LogInformation(
                                    $"[{_configKey}] Received a message, topic={message.Topic}, message-id={message.MessageId}, body-size={message.Body.Length}");
                                await _simpleConsumer.Ack(message);
                                _logger.LogInformation($"[{_configKey}] Message is acknowledged successfully, message-id={message.MessageId}");
                                // await simpleConsumer.ChangeInvisibleDuration(message, TimeSpan.FromSeconds(15));
                                // _logger.LogInformation($"Changing message invisible duration successfully, message=id={message.MessageId}");
                            }
                        }
                        else
                        {
                            await Task.Delay(TimeSpan.FromSeconds(1), _consumerCts.Token);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.LogInformation($"[{_configKey}] Consumer receive operation cancelled.");
                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"[{_configKey}] Error during message receive or handle loop.");
                        await Task.Delay(TimeSpan.FromSeconds(5), _consumerCts.Token);
                    }
                }
                _logger.LogInformation($"[{_configKey}] Consumer receive loop exited.");
            }, _consumerCts.Token);
            

            _logger.LogInformation("Run RocketMQ Consumer succeed");
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Run RocketMQ Consumer failed");
            throw;
        }
    }
    
    

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"RocketMQ Consumer [{_configKey}] stopping");
        await _consumerCts.CancelAsync();
        
        if (_simpleConsumer != null)
        {
            try
            {
                await _simpleConsumer.DisposeAsync();
                _logger.LogInformation($"RocketMQ Consumer [{_configKey}] disposed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[{_configKey}] Error during SimpleConsumer DisposeAsync.");
            }
        }

        _logger.LogInformation($"RocketMQ Consumer [{_configKey}] stopped");
        await Task.CompletedTask;
    }

    public void Dispose()
    {
        _consumerCts?.Cancel();
        _consumerCts?.Dispose();
        _simpleConsumer?.Dispose();
    }
}