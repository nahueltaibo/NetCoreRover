using Robot.Utils;

namespace Robot.MessageBus.Messages
{
    [Topic(Topics.Sensor.Switch.SinglePoleSwitch)]
    public class SwitchSensorMessage : IMessage
    {
        /// <summary>
        /// Sensor Unique Identifier.
        /// </summary>
        public int SensorId { get; set; }

        /// <summary>
        /// Switch position
        /// </summary>        
        public bool IsOn { get; set; }
    }
}
