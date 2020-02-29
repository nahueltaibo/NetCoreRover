using Iot.Device.DCMotor;
using Microsoft.Extensions.Logging;

namespace Robot.Drivers.Motors
{
    public class DCMotorDriver : IMotor
    {
        private readonly DCMotor _dcMotor;
        private readonly ILogger<DCMotorDriver> _log;

        public DCMotorDriver(DCMotor motor, ILogger<DCMotorDriver> logger)
        {
            _dcMotor = motor;
            _log = logger;
        }

        public void SetSpeed(double speed)
        {
            _log.LogDebug($"Setting MotorSpeed: {speed}");
            _dcMotor.Speed = speed;
        }
    }
}
