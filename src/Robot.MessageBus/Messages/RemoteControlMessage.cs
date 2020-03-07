using Robot.Utils;

namespace Robot.MessageBus.Messages
{
    [Topic(Topics.Command.RemoteControl)]
    public class RemoteControlMessage : IMessage
    {
        public int Key { get; set; }

        public double Value { get; set; }
    }
}
