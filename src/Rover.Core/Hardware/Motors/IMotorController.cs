namespace Rover.Core.Hardware.Motors
{
    public interface IMotorController
    {
        /// <summary>
        /// Sets the translation speed of the rover
        /// </summary>
        /// <param name="speed">Value between -1 and 1 where</param>
        /// <remarks>
        /// -1 Means full backwards speed
        /// 0 means no speed (motors will spin freely)
        /// 1 Means full forward speed
        /// </remarks>
        void SetSpeed(double speed);

        /// <summary>
        /// Sets the rotation speed of the rover
        /// </summary>
        /// <param name="rotation">Value between -1 and 1 where</param>
        /// <remarks>
        /// -1 Means full left
        /// 0 means no rotation
        /// 1 Means full right
        /// </remarks>
        void SetRotation(double rotation);
    }
}