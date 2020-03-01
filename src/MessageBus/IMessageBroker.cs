using System;

namespace MessageBus
{
    public interface IMessageBroker
    {
        void Publish<T>(T message) where T : IMessage;

        void Subscribe<T>(Action<IMessage> callback) where T : IMessage;
    }
}