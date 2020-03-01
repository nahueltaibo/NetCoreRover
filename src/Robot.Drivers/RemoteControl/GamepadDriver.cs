using Gamepad;
using Microsoft.Extensions.Logging;
using Robot.Utils;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Robot.Drivers.RemoteControl
{
    public class GamepadDriver : IGamepadDriver, IDisposable
    {
        private const string gamepadFile = "/dev/input/js0";
        private readonly ILogger<GamepadDriver> _log;
        private readonly Task _connectionTask;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private GamepadController _gamepad;
        private bool _remoteControlInitialized;

        public GamepadDriver(ILogger<GamepadDriver> logger)
        {
            _log = logger;

            _ = Task.Factory.StartNew(() => HandleGamepadConnection(_cancellationTokenSource.Token), TaskCreationOptions.LongRunning);
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

        public event EventHandler<GamepadEventArgs> KeyChanged;

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
            KeyChanged?.Invoke(this, new GamepadEventArgs
            {
                Key = e.Axis,
                Value = ValueMapper.Map(e.Value, -32767, 32767, -1, 1)
            });
        }

        private void Gamepad_ButtonChanged(object sender, ButtonEventArgs e)
        {
            KeyChanged?.Invoke(this, new GamepadEventArgs
            {
                Key = e.Button,
                Value = e.Pressed ? 1 : 0
            });
        }

        private bool RemoteControlConnected()
        {
            return File.Exists(gamepadFile);
        }

        public void Dispose()
        {
            // Cancel the task that checks for a connected gamepad
            _cancellationTokenSource.Cancel();

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
        }
    }
}
