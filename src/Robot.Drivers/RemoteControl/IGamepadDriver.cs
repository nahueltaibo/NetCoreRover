using System;

namespace Robot.Drivers.RemoteControl
{
    public interface IGamepadDriver
    {
        event EventHandler<GamepadEventArgs> KeyChanged;
    }
}
