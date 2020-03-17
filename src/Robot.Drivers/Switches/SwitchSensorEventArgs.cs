namespace Robot.Drivers.Switches
{
    public class SwitchSensorEventArgs
    {
        /// <summary>
        /// Switch Unique Identifier.
        /// </summary>
        public int SwitchId { get; set; }

        /// <summary>
        /// Switch position
        /// </summary>        
        public bool IsOn { get; set; }
    }
}