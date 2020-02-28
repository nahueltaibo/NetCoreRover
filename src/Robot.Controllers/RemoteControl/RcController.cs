using MessageBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Robot.Drivers.RemoteControl;
using Robot.MessageBus.Messages;

namespace Robot.Controllers.RemoteControl
{
    public class RCController : IRCController
    {
        private readonly IGamepadDriver _gamepadDriver;
        private readonly IMessageBroker _messageBroker;
        private readonly IOptions<RCOptions> _options;
        private readonly ILogger<RCController> _log;

        public RCController(IGamepadDriver gamepadDriver, IMessageBroker messageBroker, IOptions<RCOptions> options, ILogger<RCController> logger)
        {
            _gamepadDriver = gamepadDriver;
            _messageBroker = messageBroker;
            _options = options;
            _log = logger;

            _gamepadDriver.KeyChanged += OnGamepadKeyChanged;
        }

        private void OnGamepadKeyChanged(object sender, GamepadEventArgs e)
        {
            if (e.Key == _options.Value.GamepadTranslationAxisKey)
            {
                _messageBroker.PublishAsync(new DirectionMessage { X = e.Value });
            }

            if (e.Key == _options.Value.GamepadRotationAxisKey)
            {
                _messageBroker.PublishAsync(new DirectionMessage { Y = e.Value });
            }
        }
    }
}
