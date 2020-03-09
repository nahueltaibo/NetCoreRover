using Iot.Device.DCMotor;
using Microsoft.Extensions.Logging;
using System;

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
            _log.LogDebug($"Setting MotorSpeed: {speed:0.00}");
            _dcMotor.Speed = speed;
        }

        ~DCMotorDriver()
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
                // Stopping the motor before exit the application
                _dcMotor.Speed = 0;
                _dcMotor.Dispose();
            }
        }
    }
}
