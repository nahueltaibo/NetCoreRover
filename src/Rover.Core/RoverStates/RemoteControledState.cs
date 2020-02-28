using Gamepad;
using Microsoft.Extensions.Logging;
using Robot.Utils;
using Rover.Core.Hardware;
using Rover.Core.Hardware.Motors;
using System;
using System.IO;

namespace Rover.Core.RoverStates
{
    public class RemoteControledState : RoverState, IDisposable
    {
        private const string gamepadFile = "/dev/input/js0";
        private readonly ILogger<RemoteControledState> _log;
        private readonly MotorController _motorController;
        private GamepadController _gamepad;
        private bool _remoteControlInitialized; //Set to true when we are connected to a gamepad
        private bool _remoteControlEnabled; //Set to true when we the user enabled remote control

        public RemoteControledState(MotorController hardwareMap, IRoverStateManager roverStateManager, RoverContext context, ILogger<RemoteControledState> logger) : base(roverStateManager, context)
        {
            this._motorController = hardwareMap;
            this._log = logger;
        }

        public override void Update()
        {
            if (RemoteControlConnected() && !_remoteControlInitialized)
            {
                InitializeRemoteControl();
            }
        }

        /// <summary>
        /// Checks if the Gamepad's file descriptor exits
        /// </summary>
        /// <returns></returns>
        private bool RemoteControlConnected()
        {
            return File.Exists(gamepadFile);
        }

        /// <summary>
        /// Hooks to RC Events when RC gets connected
        /// </summary>
        private void InitializeRemoteControl()
        {
            _gamepad = new GamepadController(gamepadFile);

            _log.LogDebug("Configuring ButtonChanged listener...");
            _gamepad.ButtonChanged += Gamepad_ButtonChanged;

            _log.LogDebug("Configuring AxisChanged listener...");
            _gamepad.AxisChanged += Gamepad_AxisChanged;

            _remoteControlInitialized = true;
        }

        private void Gamepad_ButtonChanged(object sender, ButtonEventArgs e)
        {
            switch ((RemoteControlKeys)e.Button)
            {
                case RemoteControlKeys.StartButton:
                    ProcessStartButton(e);
                    break;
                case RemoteControlKeys.SpeedAxis:
                    break;
                case RemoteControlKeys.DirectionAxis:

                    break;
                default:
                    _log.LogDebug($"Button {e.Button} Pressed: {e.Pressed}");
                    break;
            }
        }

        private void Gamepad_AxisChanged(object sender, AxisEventArgs e)
        {
            switch ((RemoteControlKeys)e.Axis)
            {
                case RemoteControlKeys.SpeedAxis:
                    ProcessSpeedAxis(e);
                    break;
                case RemoteControlKeys.DirectionAxis:
                    ProcessDirectionAxis(e);
                    break;
                default:
                    _log.LogDebug($"Axis {e.Axis} Pressed: {e.Value}");
                    break;
            }
        }

        private void ProcessStartButton(ButtonEventArgs e)
        {
            if (e.Pressed && !_remoteControlEnabled)
            {
                _remoteControlEnabled = true;
                _log.LogDebug($"Remote Control is now enabled");
            }
        }

        private void ProcessDirectionAxis(AxisEventArgs e)
        {
            double mappedValue = ValueMapper.Map(e.Value, -32767, 32767, -1, 1);
            _motorController.SetSpeed(mappedValue);
        }

        private void ProcessSpeedAxis(AxisEventArgs e)
        {
            double mappedValue = ValueMapper.Map(e.Value, -32767, 32767, -1, 1) * -1; // Gamepad forward produces negative values
            _motorController.SetRotation(mappedValue);
        }

        public void Dispose()
        {
            _gamepad.Dispose();
        }
    }
}
