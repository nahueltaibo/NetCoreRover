using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Robot.MessageBus;
using Robot.MessageBus.Messages;

namespace Robot.Controllers.Motion
{
    public class ObstacleAvoidanceController : IHostedService
    {
        private readonly IMessageBroker _messageBroker;
        private readonly ILogger<ObstacleAvoidanceController> _logger;
        private readonly ObstacleAvoidanceCalculator _calculator;
        private readonly ObstacleAvoidanceControllerSettings _settings;
        private readonly Stopwatch _stopwatch = new Stopwatch();

        private CancellationTokenSource CourseCorrectionCancellationToken { get; }

        public ObstacleAvoidanceController(IMessageBroker messageBroker, ILogger<ObstacleAvoidanceController> logger,
            ObstacleAvoidanceCalculator calculator, ObstacleAvoidanceControllerSettings settings)
        {
            _messageBroker = messageBroker;
            _logger = logger;
            _calculator = calculator;
            _settings = settings;
            CourseCorrectionCancellationToken = new CancellationTokenSource();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Starting service {nameof(ObstacleAvoidanceController)}...");
            _messageBroker.Subscribe<RequestedVelocityMessage>(HandleRequestedVelocityMessage);
            _messageBroker.Subscribe<DistanceMessage>(HandleDistanceMessage);
#pragma warning disable 4014
            // Justification: We're starting a background cycle that shouldn't be awaited
            Task.Run(CourseCorrectionCycle);
#pragma warning restore 4014
            await Task.CompletedTask;
        }

        private async Task CourseCorrectionCycle()
        {
            var token = CourseCorrectionCancellationToken.Token;
            
            // Shows if we should skip the next calculation cycle
            // (due to an exceeded time budget for calculation or to an error)
            bool skipNextCycle = false;
            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (skipNextCycle)
                    {
                        skipNextCycle = false;
                        await Task.Delay(_settings.CourseCorrectionInterval);
                        continue;
                    }
                    
                    var interval = Task.Delay(_settings.CourseCorrectionInterval);
                    var calculation = CalculateCourseCorrection();
                    Task.WaitAll(interval, calculation);

                    var (calculationTime, message) = calculation.Result;

                    if (calculationTime > _settings.SkipFrameThreshold)
                    {
                        skipNextCycle = true;
                    }
                    
                    _messageBroker.Publish(message);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to correct course");
                    
                    // Making a pause after an error
                    skipNextCycle = true;
                }
            }
        }

        private Task<(TimeSpan, AppliedVelocityMessage)> CalculateCourseCorrection()
        {
            return Task.Run(() =>
            {
                _stopwatch.Restart();
                var result = _calculator.GetResultingVector();
                return (_stopwatch.Elapsed, result);
            });
        }

        private void HandleDistanceMessage(IMessage message)
        {
            if (message is DistanceMessage distanceMessage)
                _calculator.ReceiveSensorData(distanceMessage);
        }

        private void HandleRequestedVelocityMessage(IMessage message)
        {
            if (message is RequestedVelocityMessage requestedVelocityMessage)
                _calculator.ReceiveRequestedSpeed(requestedVelocityMessage);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Stopping service {nameof(ObstacleAvoidanceController)}...");
            await Task.CompletedTask;
        }
    }
}