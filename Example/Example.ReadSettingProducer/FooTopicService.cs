using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NanoRocket;
using Org.Apache.Rocketmq;

namespace Example.ReadSettingProducer;

public class FooTopicService : BackgroundService
{
    private readonly IConfiguration _configuration;

    public FooTopicService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var rocketConfig = _configuration.GetSection("NanoRocket").Get<RocketConfiguration>();
                var producerConfig = rocketConfig?.Producers ?? throw new Exception("Producers not set");
                ProducerConfig? targetConfig = new();
                producerConfig?.TryGetValue("FooTopicProducer", out targetConfig);
        
                var topic = targetConfig?.Topic ?? throw new Exception("Topic should not be null");
                var producer = await RocketProducerHelper.GetNormalProducerAsync(_configuration, "FooTopicProducer");
            
                // Define your message body.
                var bytes = Encoding.UTF8.GetBytes("foobar");
                const string tag = "yourMessageTagA";
                var message = new Message.Builder()
                    .SetTopic(topic)
                    .SetBody(bytes)
                    .SetTag(tag)
                    // You could set multiple keys for the single message actually.
                    .SetKeys("yourMessageKey-7044358f98fc")
                    .Build();

                var result = await producer.Send(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
            await Task.Delay(1000, stoppingToken);
        }
        
    }
}