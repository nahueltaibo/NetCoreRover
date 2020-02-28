namespace MessageBus.Messages
{
    [Topic("commands/speed")]
    public class SpeedMessage : IMessage
    {
        public double? X { get; set; }
        public double? Y { get; set; }
        public double? Z { get; set; }
    }
}
