using Iot.Device.DCMotor;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rover.Core.Hardware;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Rover.Core
{
    /// <summary>
    /// Main entry point for the Rover
    /// </summary>
    public class Rover : IHostedService
    {
        // Limits how often we run the main loop 100 millis means it will be ran 10 times per second 
        private const long _minLoopMillis = 100;
        private readonly ILogger<Rover> _log;
        private readonly IRoverStateManager _roverStateManager;
        private CancellationTokenSource cancellationTokenSource;

        Stopwatch stopWatch = new Stopwatch();

        public Rover(ILogger<Rover> logger, IRoverStateManager roverStateManager)
        {
            _log = logger;
            _roverStateManager = roverStateManager;
        }

        public static long MinLoopMillis => _minLoopMillis;

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
                stopWatch.Start();
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        var elapsed = stopWatch.ElapsedMilliseconds;
                        if (elapsed < _minLoopMillis)
                        {
                            Thread.Sleep((int)(_minLoopMillis - elapsed));
                        }
                        else
                        {
                            // Restart the stopwatch before running anything else, so we dont depend on
                            // how long code takes to run
                            stopWatch.Restart();

                            _roverStateManager.Update();
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.LogError(ex, ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, ex.Message);
            }
            finally
            {
                stopWatch.Stop();
            }
        }
    }
}
