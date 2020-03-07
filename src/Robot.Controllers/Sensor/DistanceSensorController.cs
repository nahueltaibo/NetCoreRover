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
        public IDistanceSensor[] Sensors { get; }
        public IMessageBroker MessageBroker { get; }
        public ILogger<DistanceSensorController> Logger { get; }

        public DistanceSensorController(IEnumerable<IDistanceSensor> sensors, IMessageBroker messageBroker, ILogger<DistanceSensorController> logger)
        {
            Sensors = sensors.ToArray();
            MessageBroker = messageBroker;
            Logger = logger;
        }

        private void InitializeSensors()
        {
            foreach (var sensor in Sensors)
            {
                sensor.SonarDistanceChanged += OnSonarDistanceChanged;
            }
        }

        private void OnSonarDistanceChanged(object sender, SonarDistanceEventArgs e)
        {
            MessageBroker.Publish(new DistanceMessage
            {
                SensorId = e.SonarId,
                SensorAngle = e.Angle,
                Distance = e.Distance
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
