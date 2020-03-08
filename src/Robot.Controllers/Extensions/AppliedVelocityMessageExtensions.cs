using System.Numerics;
using Robot.MessageBus.Messages;

namespace Robot.Controllers.Extensions
{
    public static class AppliedVelocityMessageExtensions
    {
        public static AppliedVelocityMessage ToAppliedVelocityMessage(this Vector3 vector)
        {
            return new AppliedVelocityMessage
            {
                X = vector.X,
                Y = vector.Y,
                Z = vector.Z
            };
        }
    }
}