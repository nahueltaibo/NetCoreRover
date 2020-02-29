using MessageBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Robot.Drivers.RemoteControl;
using Robot.MessageBus.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace Robot.Controllers.RemoteControl
{
    public class RemoteControlController : IRemoteControlController
    {
        private readonly IGamepadDriver _gamepadDriver;
        private readonly IMessageBroker _messageBroker;
        private readonly IOptions<RemoteControlOptions> _options;
        private readonly ILogger<RemoteControlController> _log;

        public RemoteControlController(IGamepadDriver gamepadDriver, IMessageBroker messageBroker, IOptions<RemoteControlOptions> options, ILogger<RemoteControlController> logger)
        {
            _gamepadDriver = gamepadDriver;
            _messageBroker = messageBroker;
            _options = options;
            _log = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _log.LogInformation($"Starting {nameof(RemoteControlController)}");

            _gamepadDriver.KeyChanged += OnGamepadKeyChanged;
            await Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _log.LogInformation($"Stopping {nameof(RemoteControlController)}");

            _gamepadDriver.KeyChanged -= OnGamepadKeyChanged;
            await Task.CompletedTask;
        }

        private void OnGamepadKeyChanged(object sender, GamepadEventArgs e)
        {
            if (e.Key == _options.Value.GamepadTranslationAxisKey)
            {
                _messageBroker.PublishAsync(new RemoteControlMessage { Throttle = e.Value });
            }

            if (e.Key == _options.Value.GamepadRotationAxisKey)
            {
                _messageBroker.PublishAsync(new RemoteControlMessage { Yaw = e.Value });
            }
        }
    }
}
