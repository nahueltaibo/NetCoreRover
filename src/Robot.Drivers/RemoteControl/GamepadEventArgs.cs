using Robot.Model.RemoteControl;

namespace Robot.Drivers.RemoteControl
{
    public class RemoteControlEventArgs
    {
        /// <summary>
        /// The Key that changed
        /// </summary>
        public RemoteControlKey Key { get; set; }

        /// <summary>
        /// The new value of the key
        /// -1 to 1 from a joystick (lineal control)
        /// 0 or 1 for a button (discrete control)
        /// </summary>
        public double Value { get; set; }
    }
}