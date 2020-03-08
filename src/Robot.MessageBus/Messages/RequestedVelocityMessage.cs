using Robot.Utils;

namespace Robot.MessageBus.Messages
{
    /// <summary>
    /// A class for a Velocity message sent by <see cref="Robot.Controllers.RemoteControl.RemoteControlController"/>
    /// or other input such as an AI course plotter
    /// </summary>
    /// <remarks>This velocity vector is a starting value to estimate a real robot trajectory 
    /// considering all the obstacles discovered</remarks>
    [Topic(Topics.Motion.RequestedVelocity)]
    public class RequestedVelocityMessage : AbstractVectorMessage
    {
    }
}
