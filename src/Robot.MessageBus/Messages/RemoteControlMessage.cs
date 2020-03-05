namespace Robot.MessageBus.Messages
{
    [Topic("command/remotecontrol")]
    public class RemoteControlMessage : IMessage
    {
        public int Key { get; set; }

        public double Value { get; set; }
    }
}
