using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Org.Apache.Rocketmq;

namespace NanoRocket;

public static class RocketProducerHelper
{
    public static async Task<Producer.Builder> GetProducerBuilder(IConfiguration configuration, string producerKey)
    {
        var rocketConfig = configuration.GetSection("NanoRocket");
        var producerConfig = rocketConfig.GetSection("Producers");
        var targetConfig = producerConfig.GetSection(producerKey);
        
        var topic = targetConfig.GetValue<string>("Topic") ?? throw new Exception("Topic should not be null");
        var endpoints = targetConfig.GetValue<string>("Endpoints") ?? throw new Exception("Endpoints should not be null");
        var accessKey = targetConfig.GetValue<string>("AccessKey");
        var secretKey = targetConfig.GetValue<string>("SecretKey");
        
        var credentialsProvider = !string.IsNullOrEmpty(accessKey) && !string.IsNullOrEmpty(secretKey) ? new StaticSessionCredentialsProvider(accessKey, secretKey) : null;
        var clientConfigBuilder = new ClientConfig.Builder()
            .SetEndpoints(endpoints);
        if (credentialsProvider != null) clientConfigBuilder.SetCredentialsProvider(credentialsProvider);
        var clientConfig = clientConfigBuilder.Build();

        var producerBuilder = new Producer.Builder()
            // Set the topic name(s), which is optional but recommended.
            // It makes producer could prefetch the topic route before message publishing.
            .SetTopics(topic)
            .SetClientConfig(clientConfig);
        
        return producerBuilder;
    }
    
    public static async Task<Producer> GetNormalProducerAsync(IConfiguration configuration, string producerKey)
    {
        var producerBuilder = await GetProducerBuilder(configuration, producerKey);
        var producer = await producerBuilder.Build();
        
        return producer;
    }

    // public static Task<Producer> GetTransactionProducerAsync(ref ITransaction transaction, IConfiguration configuration, string producerKey)
    // {
    //     var producerBuilder = GetProducerBuilder(configuration, producerKey).ConfigureAwait(false).GetAwaiter().GetResult();
    //     producerBuilder.SetTransactionChecker(new TransactionChecker());
    //     var producer = producerBuilder.Build().ConfigureAwait(false).GetAwaiter().GetResult();
    //
    //     transaction = producer.BeginTransaction();
    //     
    //     return Task.FromResult(producer);
    // }

    public static async Task<ISendReceipt> SendMessageAsync(Producer producer, Message message)
    {
        ISendReceipt sendReceipt;

        try
        {
            sendReceipt = await producer.Send(message);
        }
        finally
        {
            await producer.DisposeAsync();
        }

        return sendReceipt;
    }
}