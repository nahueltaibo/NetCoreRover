using System;

namespace Robot.Drivers.RemoteControl
{
    public interface IGamepadDriver : IDisposable
    {
        event EventHandler<GamepadEventArgs> KeyChanged;
    }
}
