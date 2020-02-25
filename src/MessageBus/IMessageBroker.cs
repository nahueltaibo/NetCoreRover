using System;
using System.Threading.Tasks;

namespace MessageBus.Messages
{
    public interface IMessageBroker
    {
        Task PublishAsync<T>(string topic, T message);

        Task SubscribeAsync<T>(string topic, Action<IMessage> callback);
    }
}