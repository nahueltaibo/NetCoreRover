using Iot.Device.DCMotor;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rover.Core
{
    /// <summary>
    /// Main entry point for the Rover
    /// </summary>
    public class Rover : IHostedService
    {
        private readonly ILogger<Rover> _log;
        private readonly IRoverStateManager _roverStateManager;
        private CancellationTokenSource cancellationTokenSource;

        private readonly Iot.Device.MotorHat.MotorHat motorHat;
        private readonly DCMotor leftMotor;
        private readonly DCMotor rightMotor;

        public Rover(ILogger<Rover> logger, IRoverStateManager roverStateManager)
        {
            _log = logger;
            _roverStateManager = roverStateManager;

            try
            {
                //_log.LogDebug("Creating MotorHat...");
                //// Create the MotorHat (provides access to DCMotors/StepperMotors/servoMotors/PWM channels)
                //motorHat = new Iot.Device.MotorHat.MotorHat();

                //// Create the left and right motors
                //_log.LogDebug("Creating Motors...");
                //leftMotor = motorHat.CreateDCMotor(1);
                //rightMotor = motorHat.CreateDCMotor(3);

                //// In case the process closed unexpectedly, stop both motors
                //_log.LogDebug("Stopping Motors...");
                //leftMotor.Speed = rightMotor.Speed = 0;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, ex.Message);

                try
                {
                    _log.LogDebug("Trying to stop the motors..");
                    leftMotor.Speed = rightMotor.Speed = 0;
                }
                catch (Exception motorsEx)
                {
                    _log.LogError(motorsEx, "Unable to stop the motors");
                    throw;
                }

                throw;
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            cancellationTokenSource = new CancellationTokenSource();

            _ = Task.Factory.StartNew(() => Run(cancellationTokenSource.Token), TaskCreationOptions.LongRunning);

            await Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            cancellationTokenSource.Cancel();

            await Task.CompletedTask;
        }

        private void Run(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    _roverStateManager.Update();
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }

            //try
            //{
            //    const double Period = 10.0;
            //    Stopwatch sw = Stopwatch.StartNew();
            //    string lastSpeedDisp = null;

            //    while (!token.IsCancellationRequested)
            //    {
            //        double time = sw.ElapsedMilliseconds / 1000.0;

            //        // Note: range is from -1 .. 1 (for 1 pin setup 0 .. 1)
            //        var speed = Math.Sin(2.0 * Math.PI * time / Period);
            //        leftMotor.Speed = rightMotor.Speed = speed;

            //        string disp = $"Speed = {speed:0.00}";
            //        if (disp != lastSpeedDisp)
            //        {
            //            lastSpeedDisp = disp;
            //            log.LogTrace(disp);
            //        }

            //        Thread.Sleep(1);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    log.LogError(ex, ex.Message);
            //    throw;
            //}
            //finally
            //{
            //    try
            //    {
            //        log.LogDebug("Trying to stop the motors..");
            //        leftMotor.Speed = rightMotor.Speed = 0;
            //    }
            //    catch (Exception motorsEx)
            //    {
            //        log.LogError(motorsEx, "Unable to stop the motors");
            //        throw;
            //    }
            //}
        }
    }
}
