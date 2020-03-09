using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Robot.Drivers.RemoteControl;
using Robot.MessageBus;
using Robot.MessageBus.Messages;
using Robot.Model.RemoteControl;
using System.Threading;
using System.Threading.Tasks;

namespace Robot.Controllers.RemoteControl
{
    public class RemoteControlController : IRemoteControlController
    {
        private readonly IRemoteControlDriver _gamepadDriver;
        private readonly IMessageBroker _messageBroker;
        private readonly IOptions<RemoteControlOptions> _options;
        private readonly ILogger<RemoteControlController> _log;

        public RemoteControlController(IRemoteControlDriver gamepadDriver, IMessageBroker messageBroker, IOptions<RemoteControlOptions> options, ILogger<RemoteControlController> logger)
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
            _gamepadDriver.KeyChanged -= OnGamepadKeyChanged;

            _gamepadDriver.Dispose();

            _log.LogInformation($"{nameof(RemoteControlController)} stopped");

            await Task.CompletedTask;
        }

        private void OnGamepadKeyChanged(object sender, RemoteControlEventArgs e)
        {
            if (e.Key != RemoteControlKey.Invalid)
            {
                _messageBroker.Publish(new RemoteControlMessage
                {
                    Key = (int)e.Key,
                    Value = e.Value
                });
            }
        }
    }
}
