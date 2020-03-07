using Robot.Utils;

namespace Robot.MessageBus.Messages
{
    [Topic(Topics.Motion.Velocity)]
    public class VelocityMessage : IMessage
    {
        public double? X { get; set; }
        public double? Y { get; set; }
        public double? Z { get; set; }
    }
}
