namespace MessageBus.Messages
{

    [Topic("commands/speed")]
    public class SpeedMessage : IMessage
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
    }
}
