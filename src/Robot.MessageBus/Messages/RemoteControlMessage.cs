namespace Robot.MessageBus.Messages
{
    [Topic("command/remotecontrol")]
    public class RemoteControlMessage : IMessage
    {
        /// <summary>
        /// To increase, push the left stick forwards. To decrease, pull the left stick backwards. This adjusts the altitude, or height, of the robot.
        /// </summary>
        public double? Throttle { get; set; }

        /// <summary>
        /// Pushing the right stick forwards or backwards. Tilts the robot, which maneuvers the robot forwards or backwards.
        /// </summary>
        public double? Pitch { get; set; }

        /// <summary>
        /// Pushing the left stick to the left or to the right. 
        /// Rotates the robot left or right. Points the front of the robot different directions and helps with changing directions while flying.
        /// </summary>
        public double? Yaw { get; set; }

        /// <summary>
        /// Pushing the right stick to the left or right. Rolls the robot, which maneuvers the quadcopter left or right.
        /// </summary>
        public double? Roll { get; set; }
    }
}
