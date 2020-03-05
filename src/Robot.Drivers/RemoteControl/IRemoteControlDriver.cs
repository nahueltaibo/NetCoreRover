using System;

namespace Robot.Drivers.RemoteControl
{
    public interface IRemoteControlDriver : IDisposable
    {
        event EventHandler<RemoteControlEventArgs> KeyChanged;
    }
}
