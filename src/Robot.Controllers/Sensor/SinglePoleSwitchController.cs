using Microsoft.Extensions.Logging;
using Robot.Drivers.Switches;
using Robot.MessageBus;
using Robot.MessageBus.Messages;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Robot.Controllers.Sensor
{
    /// <summary></summary>
    /// <seealso cref="Robot.Controllers.Sensor.ISinglePoleSwitchController" />
    public class SinglePoleSwitchController : ISinglePoleSwitchController
    {
        private ISwitchSensor[] Sensors { get; }
        private IMessageBroker MessageBroker { get; }
        private ILogger<SinglePoleSwitchController> Logger { get; }

        public SinglePoleSwitchController(IEnumerable<ISwitchSensor> sensors, IMessageBroker messageBroker, ILogger<SinglePoleSwitchController> logger)
        {
            Sensors = sensors.ToArray();
            MessageBroker = messageBroker;
            Logger = logger;
        }

        private void InitializeSensors()
        {
            foreach (var sensor in Sensors)
            {
                sensor.SwitchPositionChanged += OnSwitchPositionChanged;
            }
        }

        private void OnSwitchPositionChanged(object sender, SwitchSensorEventArgs e)
        {
            MessageBroker.Publish(new SwitchSensorMessage
            {
                SensorId = e.SwitchId,
                On = e.On
            });
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            InitializeSensors();
            await Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }
    }
}
