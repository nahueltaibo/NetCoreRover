using System;

namespace Robot.Drivers.Motors
{
    public interface IMotor : IDisposable
    {
        /// <summary>
        /// Sets the speed of a motor
        /// </summary>
        /// <param name="speed">Value between -1 and 1 meaning:
        /// -1 full thrust counterclockwise
        /// 0 stopped
        /// 1 full thrust clockwise</param>
        void SetSpeed(double speed);
    }
}
