using System;

namespace Robot.Controllers.Motion
{
    /// <summary>
    /// Class that represents <see cref="ObstacleAvoidanceController"/> settings
    /// </summary>
    public class ObstacleAvoidanceControllerSettings
    {
        /// <summary>
        /// Gets or sets a threshold to determine if the Controller should skip the next calculation cycle.
        /// </summary>
        /// <remarks>Should be less than <see cref="CourseCorrectionInterval"/></remarks>
        public TimeSpan SkipFrameThreshold { get; set; } = TimeSpan.FromMilliseconds(150);
        
        /// <summary>
        /// Gets or sets Course correction interval.
        /// </summary>
        /// <remarks>Should be greater than <see cref="SkipFrameThreshold"/></remarks>
        public TimeSpan CourseCorrectionInterval { get; set; } = TimeSpan.FromMilliseconds(200);
    }
}