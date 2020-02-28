using Iot.Device.DCMotor;
using Iot.Device.MotorHat;
using MessageBus;
using MessageBus.Messages;
using Microsoft.Extensions.Logging;
using Robot.Utils;
using System;
using System.Numerics;

namespace Rover.Core.Hardware.Motors
{
    /// <summary>
    /// Represents the hardware available in the robot.
    /// </summary>
    public class MotorController : IMotorController, IDisposable
    {
        private readonly IMessageBroker _messageBus;
        private readonly ILogger<MotorController> _log;
        private MotorHat _motorHat;
        private readonly DCMotor _leftMotor;
        private readonly DCMotor _rightMotor;

        private double _translation;
        private double _rotation;

        public MotorController(IMessageBroker messageBus, ILogger<MotorController> logger)
        {
            _log = logger;
            _messageBus = messageBus;

            _log.LogDebug("Creating MotorHat...");
            // Create the MotorHat (provides access to DCMotors/StepperMotors/servoMotors/PWM channels)
            _motorHat = new MotorHat();

            // Create the left and right motors
            _log.LogDebug("Creating Motors...");
            _leftMotor = _motorHat.CreateDCMotor(1);
            _rightMotor = _motorHat.CreateDCMotor(3);

            // In case the process closed unexpectedly, stop both motors
            _log.LogDebug("Stopping Motors...");
            _leftMotor.Speed = _rightMotor.Speed = 0;

            // Subscribe to SpeedMessages
            _messageBus.SubscribeAsync<SpeedMessage>(OnSpeedReceived).Wait();
        }

        private void OnSpeedReceived(SpeedMessage speedMessage)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _motorHat.Dispose();
        }

        public void SetSpeed(double speed)
        {
            _translation = speed;

            UpdateMotors();
        }

        public void SetRotation(double rotation)
        {
            _rotation = rotation;

            UpdateMotors();
        }

        private void UpdateMotors()
        {
            var result = JoystickToDiff(_translation, _rotation, -1, 1, -1, 1);

            _log.LogDebug($"[Motors] left: {result.X}; right: {result.Y}");
            _leftMotor.Speed = result.X;
            _rightMotor.Speed = result.Y;
        }

        private Vector2 JoystickToDiff(double x, double y, double minJoystick, double maxJoystick, double minSpeed, double maxSpeed)
        {
            // If x and y are 0, then there is not much to calculate...
            if (x == 0 && y == 0)
            {
                return new Vector2(0, 0);
            }

            // First Compute the angle in deg
            // First hypotenuse
            var z = Math.Sqrt(x * x + y * y);

            // angle in radians
            var rad = Math.Acos(Math.Abs(x) / z);

            // and in degrees
            var angle = rad * 180 / Math.PI;

            // Now angle indicates the measure of turn
            // Along a straight line, with an angle o, the turn co-efficient is same
            // this applies for angles between 0-90, with angle 0 the coeff is -1
            // with angle 45, the co-efficient is 0 and with angle 90, it is 1

            var tcoeff = -1 + (angle / 90) * 2;
            var turn = tcoeff * Math.Abs(Math.Abs(y) - Math.Abs(x));
            turn = Math.Round(turn * 100, 0) / 100;

            // And max of y or x is the movement
            var mov = Math.Max(Math.Abs(y), Math.Abs(x));


            double rawLeft, rawRight;

            // First and third quadrant
            if ((x >= 0 && y >= 0) || (x < 0 && y < 0))
            {
                rawLeft = mov;
                rawRight = turn;
            }
            else
            {
                rawRight = mov;
                rawLeft = turn;
            }

            // Reverse polarity
            if (y < 0)
            {
                rawLeft = 0 - rawLeft;
                rawRight = 0 - rawRight;
            }

            // minJoystick, maxJoystick, minSpeed, maxSpeed
            // Map the values onto the defined rang
            float rightOut = (float)ValueMapper.Map(rawRight, minJoystick, maxJoystick, minSpeed, maxSpeed);
            float leftOut = (float)ValueMapper.Map(rawLeft, minJoystick, maxJoystick, minSpeed, maxSpeed);

            return new Vector2(rightOut, leftOut);
        }
    }
}
