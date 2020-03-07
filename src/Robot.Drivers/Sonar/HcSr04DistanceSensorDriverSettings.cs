using System;

namespace Robot.Drivers.Sonar
{
    public class HcSr04DistanceSensorDriverSettings
    {
        /// <summary>
        /// Unique ID to refer this sensor.
        /// </summary>
        public int SensorId { get; set; }

        /// <summary>
        /// Sensor angle in radians.
        /// </summary>
        /// <remarks>Zero is a forward direction of a Robot. Pi/2 is a right side, -Pi/2 is a left side</remarks>
        public double Angle { get; set; }

        /// <summary>
        /// GPIO Trigger PIN number.
        /// </summary>
        public int TriggerPin { get; set; }

        /// <summary>
        /// GPIO Echo PIN number.
        /// </summary>
        public int EchoPin { get; set; }

        /// <summary>
        /// Distance measuring interval.
        /// </summary>
        public TimeSpan MeasuringInterval { get; set; }
    }
}