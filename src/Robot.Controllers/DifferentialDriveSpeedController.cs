using System;
using System.Numerics;
using MessageBus;
using MessageBus.Messages;
using Microsoft.Extensions.Logging;
using Robot.Drivers;
using Robot.Utils;

namespace Robot.Controllers
{
    public class DifferentialDriveSpeedController : ISpeedController
    {
        private readonly IMotor _leftMotor;
        private readonly IMotor _rightMotor;
        private readonly IMessageBroker _messageBroker;
        private readonly ILogger<DifferentialDriveSpeedController> _log;
        private double _translation = 0;
        private double _rotation = 0;

        public DifferentialDriveSpeedController(IMotor leftMotor, IMotor rightMotor, IMessageBroker messageBroker, ILogger<DifferentialDriveSpeedController> logger)
        {
            _leftMotor = leftMotor;
            _rightMotor = rightMotor;
            _messageBroker = messageBroker;
            _log = logger;

            // Subscribe to SpeedMessages...
            _messageBroker.SubscribeAsync<SpeedMessage>(OnSpeedMessageReceived);
        }

        private void OnSpeedMessageReceived(SpeedMessage speedMessage)
        {
            // Update the direction and rotation only if we are receiving a valid value
            _translation = speedMessage.X ?? _translation;
            _rotation = speedMessage.Y ?? _rotation;

            // Convert translation and rotation to the speeds to be set to each motor
            var motorSpeeds = JoystickToDiff(_translation, _rotation, -1, 1, -1, 1);

            // Set the speeds to each motor
            _leftMotor.SetSpeed(motorSpeeds.Item1);
            _rightMotor.SetSpeed(motorSpeeds.Item2);
        }

        private Tuple<double, double> JoystickToDiff(double x, double y, double minJoystick, double maxJoystick, double minSpeed, double maxSpeed)
        {
            // If x and y are 0, then there is not much to calculate...
            if (x == 0 && y == 0)
            {
                return new Tuple<double, double>(0, 0);
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
            var leftOut = ValueMapper.Map(rawLeft, minJoystick, maxJoystick, minSpeed, maxSpeed);
            var rightOut = ValueMapper.Map(rawRight, minJoystick, maxJoystick, minSpeed, maxSpeed);

            return new Tuple<double, double>(leftOut, rightOut);
        }
    }
}
