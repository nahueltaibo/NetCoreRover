namespace Robot.Drivers.Sonar
{
    public class SonarDistanceEventArgs
    {
        /// <summary>
        /// Sensor Unique Identifier.
        /// </summary>
        public int SonarId { get; set; }

        /// <summary>
        /// Sensor angle in radians.
        /// </summary>
        /// <remarks>Zero is a forward direction of a Robot. Pi/2 is a right side, -Pi/2 is a left side</remarks>
        public double Angle { get; set; }

        /// <summary>
        /// Distance to an obstacle in meters
        /// </summary>
        public double? Distance { get; set; }
    }
}