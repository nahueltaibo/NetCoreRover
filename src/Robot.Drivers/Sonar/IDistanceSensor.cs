using System;

namespace Robot.Drivers.Sonar
{
    public interface IDistanceSensor : IDisposable
    {
        /// <summary>
        /// Sensor Unique Identifier.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Sensor angle in radians.
        /// </summary>
        /// <remarks>Zero is a forward direction of a Robot. Pi/2 is a right side, -Pi/2 is a left side</remarks>
        double Angle { get; }

        /// <summary>
        /// Event raised after each measurement.
        /// </summary>
        event EventHandler<SonarDistanceEventArgs> SonarDistanceChanged;
    }
}
