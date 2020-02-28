using System;

namespace MessageBus
{
    internal class Subscription
    {
        public Type Type { get; internal set; }
        public string Topic { get; internal set; }
        public object Callback { get; internal set; }
    }
}