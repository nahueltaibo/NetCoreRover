using Robot.Utils;

namespace Robot.MessageBus.Messages
{
    /// <summary>
    /// A class for a Velocity message sent by <see cref="Robot.Controllers.Motion.ObstacleAvoidanceController"/>
    /// </summary>
    /// <remarks>This velocity vector is a final robot trajectory considering all the obstacles discovered</remarks>
    [Topic(Topics.Motion.AppliedVelocity)]
    public class AppliedVelocityMessage : AbstractVectorMessage
    {
    }
}
