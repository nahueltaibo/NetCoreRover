using System;

namespace Robot.Drivers.Switches
{
    public class SwitchSensorDriverSettings
    {
        /// <summary>
        /// Unique ID to refer this Switch Sensor.
        /// </summary>
        public int SensorId { get; set; }


        /// <summary>
        /// GPIO Trigger PIN number.
        /// </summary>
        public int SwitchPin { get; set; }

        /// <summary>
        /// Distance measuring interval.
        /// </summary>
        public TimeSpan MeasuringInterval { get; set; }
    }
}