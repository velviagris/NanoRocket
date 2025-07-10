using Org.Apache.Rocketmq;

namespace NanoRocket;

public interface IMessageHandler
{
    public Task HandleMessage(MessageView message);
}
