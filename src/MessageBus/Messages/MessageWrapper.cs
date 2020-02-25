namespace MessageBus.Messages
{
    internal class MessageWrapper
    {
        public string Payload { get; internal set; }
        public string Type { get; internal set; }
    }
}