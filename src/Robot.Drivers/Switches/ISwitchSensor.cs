using System;

namespace Robot.Drivers.Switches
{
    /// <summary></summary>
    /// Basic Open/Close switch sensor
    /// <seealso cref="System.IDisposable" />
    public interface ISwitchSensor : IDisposable
    {
        /// <summary>
        /// Switch Unique Identifier.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Switch position
        /// </summary>        
        bool IsOn { get; }

        /// <summary>
        /// Event triggered on toggle switch position
        /// </summary>
        event EventHandler<SwitchSensorEventArgs> SwitchPositionChanged;
    }
}
