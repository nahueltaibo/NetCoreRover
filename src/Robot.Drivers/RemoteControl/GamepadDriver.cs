using Gamepad;
using Microsoft.Extensions.Logging;
using Robot.Model.RemoteControl;
using Robot.Utils;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Robot.Drivers.RemoteControl
{
    public class GamepadDriver : IRemoteControlDriver
    {
        private const string gamepadFile = "/dev/input/js0";
        private readonly ILogger<GamepadDriver> _log;
        private readonly Task _connectionTask;
        private readonly CancellationTokenSource _detectingGamepad = new CancellationTokenSource();
        private GamepadController _gamepad;
        private bool _remoteControlInitialized;

        public GamepadDriver(ILogger<GamepadDriver> logger)
        {
            _log = logger;

            _connectionTask = Task.Factory.StartNew(() => HandleGamepadConnection(_detectingGamepad.Token), TaskCreationOptions.LongRunning);
        }

        private void HandleGamepadConnection(CancellationToken token)
        {
            try
            {
                while (true)
                {
                    token.ThrowIfCancellationRequested();

                    if (RemoteControlConnected() && !_remoteControlInitialized)
                    {
                        _log.LogDebug($"Gamepad detected, connecting to it...");

                        InitializeRemoteControl();

                        _remoteControlInitialized = true;

                        _log.LogWarning($"Gamepad connected.");
                    }
                    else if (!RemoteControlConnected() && _remoteControlInitialized)
                    {
                        _log.LogWarning($"Gamepad disconected.");

                        // If the gamepad file doens't exist any more, clear the initialized flag,
                        // So when gamepad becomes available again we reconnect to it
                        _remoteControlInitialized = false;
                        _gamepad = null;
                    }

                    Thread.Sleep(1000);
                }
            }
            catch (TaskCanceledException)
            {
                _log.LogDebug("Gamepad connection task correctly cancelled");
            }
            catch (Exception ex)
            {
                _log.LogError($"Unexpected error in GamepadConnection task", ex);
            }
        }

        public event EventHandler<RemoteControlEventArgs> KeyChanged;

        private void InitializeRemoteControl()
        {
            _gamepad = new GamepadController(gamepadFile);

            _log.LogDebug("Configuring ButtonChanged listener...");
            _gamepad.ButtonChanged += Gamepad_ButtonChanged;

            _log.LogDebug("Configuring AxisChanged listener...");
            _gamepad.AxisChanged += Gamepad_AxisChanged;
        }

        private void Gamepad_AxisChanged(object sender, AxisEventArgs e)
        {
            var rcKey = MapToRemoteControlKey(e.Axis, true);

            if (rcKey != RemoteControlKey.Invalid)
            {
                var value = ValueMapper.Map(e.Value, -32767, 32767, -1, 1) * ((rcKey == RemoteControlKey.Throttle || rcKey == RemoteControlKey.Yaw) ? -1 : 1);

                KeyChanged?.Invoke(this, new RemoteControlEventArgs
                {
                    Key = rcKey,
                    // Gamepad's Throttle and Yaw values are inverted.. fix that before sending
                    Value = value
                });
            }
        }

        private void Gamepad_ButtonChanged(object sender, ButtonEventArgs e)
        {
            var rcKey = MapToRemoteControlKey(e.Button, false);
            if (rcKey != RemoteControlKey.Invalid)
            {
                KeyChanged?.Invoke(this, new RemoteControlEventArgs
                {
                    Key = rcKey,
                    Value = e.Pressed ? 1 : 0
                });
            }
        }

        private RemoteControlKey MapToRemoteControlKey(int code, bool isAxis)
        {
            if (isAxis)
            {
                // Handle Axis
                switch (code)
                {
                    case 0: return RemoteControlKey.Yaw;
                    case 1: return RemoteControlKey.Throttle;
                    case 2: return RemoteControlKey.Roll;
                    case 3: return RemoteControlKey.Pitch;
                }
            }
            else
            {
                // Handle buttons
                switch (code)
                {
                    case 0: return RemoteControlKey.A;
                    case 1: return RemoteControlKey.B;
                    case 3: return RemoteControlKey.X;
                    case 4: return RemoteControlKey.Y;
                    case 6: return RemoteControlKey.L1;
                    case 7: return RemoteControlKey.R1;
                    case 8: return RemoteControlKey.L2;
                    case 9: return RemoteControlKey.R2;
                    case 10: return RemoteControlKey.Select;
                    case 11: return RemoteControlKey.Start;
                }
            }

            return RemoteControlKey.Invalid;
        }

        private bool RemoteControlConnected()
        {
            return File.Exists(gamepadFile);
        }

        ~GamepadDriver()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Cancel the task that checks for a connected gamepad
                _detectingGamepad.Cancel();

                try
                {
                    _connectionTask.Wait();
                }
                catch (TaskCanceledException)
                {
                }
                catch (Exception ex)
                {
                    _log.LogError($"Unexpected exception when cancelling the Gamepad connection Task", ex);
                    throw;
                }
                _log.LogDebug($"{nameof(GamepadDriver)} stopped");
            }
        }

    }
}
