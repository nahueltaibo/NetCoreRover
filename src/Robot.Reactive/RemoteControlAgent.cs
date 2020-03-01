using MessageBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Robot.MessageBus.Messages;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Robot.Reactive
{
    public class RemoteControlAgent : IHostedService
    {
        // Limits how often we run the main loop 100 millis means it will be ran 10 times per second 
        private const long _minLoopMillis = 100;
        private readonly IMessageBroker _messageBroker;
        private readonly ILogger<RemoteControlAgent> _log;
        private CancellationTokenSource _cancellationTokenSource;
        double _currentTranslation = 0;
        double _currentRotation = 0;

        // We will only publish messages if there is any change (And when we start, so we stop the motors in case they are running)
        bool _changed = true;

        Stopwatch stopWatch = new Stopwatch();

        public RemoteControlAgent(IMessageBroker messageBroker, ILogger<RemoteControlAgent> logger)
        {
            _messageBroker = messageBroker;
            _log = logger;
        }

        private void OnRemoteControlMessageReceived(IMessage message)
        {
            var directionMessage = message as RemoteControlMessage;

            if (directionMessage.Throttle.HasValue || directionMessage.Yaw.HasValue)
            {
                _currentTranslation = directionMessage.Throttle ?? _currentTranslation;
                _currentRotation = directionMessage.Yaw ?? _currentRotation;
                _changed = true;
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _log.LogInformation($"Starting {nameof(RemoteControlAgent)}");

            _cancellationTokenSource = new CancellationTokenSource();

            _ = Task.Factory.StartNew(() => Run(_cancellationTokenSource.Token), TaskCreationOptions.LongRunning);

            _messageBroker.Subscribe<RemoteControlMessage>(OnRemoteControlMessageReceived);

            await Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _log.LogInformation($"Stopping {nameof(RemoteControlAgent)}");

            _cancellationTokenSource.Cancel();

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

                            // Run whatever we need...
                            if (_changed)
                            {
                                _messageBroker.Publish(new VelocityMessage
                                {
                                    X = _currentTranslation,
                                    Y = _currentRotation,
                                    Z = 0
                                });

                                _changed = false;
                            }
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
