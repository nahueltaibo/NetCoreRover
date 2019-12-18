using Microsoft.Extensions.Logging;

namespace Rover.Core.RoverStates
{
    public class AutonomuseState : RoverState
    {
        private readonly ILogger<AutonomuseState> _log;

        public AutonomuseState(IRoverStateManager roverStateManager, RoverContext context, ILogger<AutonomuseState> logger) : base(roverStateManager, context)
        {
            this._log = logger;
        }

        public override void Update()
        {
            _log.LogDebug("Updating State...");

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
