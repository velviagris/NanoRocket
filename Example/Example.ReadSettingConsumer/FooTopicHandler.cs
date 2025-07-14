using System.Text;
using NanoRocket;
using Org.Apache.Rocketmq;

namespace Example.ReadSettingConsumer;

public class FooTopicHandler : IMessageHandler
{
    public async Task HandleMessageAsync(MessageView message)
    {
        Console.WriteLine(Encoding.UTF8.GetString(message.Body));

        await Task.Delay(1000);
    }
}