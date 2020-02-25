using System;

namespace MessageBus
{
    internal class Subscription
    {
        public Type Type { get; internal set; }
        public string Topic { get; internal set; }
        public Action<IMessage> Callback { get; internal set; }
    }
}