using MessageBus;

namespace Robot.MessageBus.Messages
{
    [Topic("motion/velocity")]
    public class VelocityMessage : MessageBase, IMessage
    {
        public double? X { get; set; }
        public double? Y { get; set; }
        public double? Z { get; set; }
    }
}
