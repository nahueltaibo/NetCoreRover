using Robot.Utils;

namespace Robot.MessageBus.Messages
{
    public abstract class AbstractVectorMessage : IMessage
    {
        public double? X { get; set; }
        public double? Y { get; set; }
        public double? Z { get; set; }
    }
}
