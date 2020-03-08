namespace Robot.Controllers.Motion
{
    /// <summary>
    /// Class representing <see cref="ObstacleAvoidanceCalculator"/> settings
    /// </summary>
    public class ObstacleAvoidanceCalculatorSettings
    {
        /// <summary>
        /// Gets or sets how many messages should Calculator receive from random sensors till it's
        /// sure that it received messages from each sensor at least once
        /// </summary>
        public int MessagesToBecomeOperational { get; set; } = 50;

        /// <summary>
        /// Gets or sets the distance in meters of the maximum distance where the field starts to affect a
        /// robot's movement
        /// </summary>
        /// <remarks>A default value is 0.3, which is 30 cm</remarks>
        public float DistanceThreshold { get; set; } = 0.3f;

        /// <summary>
        /// Gets or sets the distance in meters of the minimum distance, starting from which the field affects
        /// robot's movement at a maximum level
        /// </summary>
        /// <remarks>A default value is 0.20, which is 20% of a <see cref="DistanceThreshold"/></remarks>
        public float MinimalDistancePercent { get; set; } = 0.2f;
    }
}