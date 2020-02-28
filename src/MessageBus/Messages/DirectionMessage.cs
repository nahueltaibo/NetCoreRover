using MessageBus;

namespace Robot.MessageBus.Messages
{
    [Topic("commands/direction")]
    public class DirectionMessage : IMessage
    {
        public double? X { get; set; }

        public double? Y { get; set; }

        public double? Z { get; set; }
    }
}
