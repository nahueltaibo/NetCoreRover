using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Robot.Drivers.Sonar;
using Robot.MessageBus;
using Robot.MessageBus.Messages;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Robot.Controllers.Sensor
{
    public class DistanceSensorController : IHostedService, IDistanceSensorController
    {
        private readonly IDistanceSensor[] _sensors;
        private readonly IMessageBroker _messageBroker;
        private readonly ILogger<DistanceSensorController> _logger;

        public DistanceSensorController(IEnumerable<IDistanceSensor> sensors, IMessageBroker messageBroker, ILogger<DistanceSensorController> logger)
        {
            _sensors = sensors.ToArray();
            _messageBroker = messageBroker;
            _logger = logger;
        }

        private void InitializeSensors()
        {
            foreach (var sensor in _sensors)
            {
                sensor.SonarDistanceChanged += OnSonarDistanceChanged;
            }
        }

        private void OnSonarDistanceChanged(object sender, SonarDistanceEventArgs e)
        {
            _messageBroker.Publish(new DistanceMessage
            {
                SensorId = e.SonarId,
                SensorAngle = e.Angle,
                Distance = e.Distance
            });
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Starting service {nameof(DistanceSensorController)}...");
            InitializeSensors();
            await Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Stopping service {nameof(DistanceSensorController)}...");
            await Task.CompletedTask;
        }
    }
}
