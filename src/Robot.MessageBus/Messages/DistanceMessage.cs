using Robot.Utils;

namespace Robot.MessageBus.Messages
{
    [Topic(Topics.Sensor.Sonar.Distance)]
    public class DistanceMessage : IMessage
    {
        /// <summary>
        /// Sensor Unique Identifier.
        /// </summary>
        public int SensorId { get; set; }

        /// <summary>
        /// Sensor angle in radians.
        /// </summary>
        /// <remarks>Zero is a forward direction of a Robot. Pi/2 is a right side, -Pi/2 is a left side</remarks>
        public double SensorAngle { get; set; }

        /// <summary>
        /// Distance to an obstacle in meters
        /// </summary>
        public double? Distance { get; set; }
    }
}
