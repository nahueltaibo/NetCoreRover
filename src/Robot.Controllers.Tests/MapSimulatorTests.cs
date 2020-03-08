using System;
using System.Numerics;
using NUnit.Framework;
using Robot.Controllers.Extensions;
using Robot.Controllers.Motion;
using Robot.Controllers.Tests.Infrastructure;
using Robot.MessageBus.Messages;

namespace Robot.Controllers.Tests
{
    [TestFixture]
    public class MapSimulatorTests
    {
        private const float DistanceThreshold = 0.3f;
        private const float Pi = 3.14159265358979323846f;
        
        [TestCase(0, 0, 0.1f, true)]
        [TestCase(0, Pi / 6, 0.1f, false)]
        [TestCase(0, - Pi / 6, 0.1f, false)]
        [TestCase(0, Pi / 8, 0.1f, false)]
        [TestCase(0, - Pi / 8, 0.1f, false)]
        [TestCase(Pi / 6, Pi / 6, 0.1f, true)]
        [TestCase(0, Pi, 0.1f, false)]
        [TestCase(0, 0, 0.0f, false)]
        public void SensorVisibilityTest(float sensorAngle, float obstacleAngle, float distance, bool shouldBeVisible)
        {
            var underTest = BuildMapSimulator(sensorAngle, obstacleAngle, distance);
            
        }

        private MapSimulator BuildMapSimulator(float sensorAngle, float obstacleAngle, float distance)
        {
            Calculator = new ObstacleAvoidanceCalculator(new ObstacleAvoidanceCalculatorSettings
            {
                DistanceThreshold = DistanceThreshold,
                MessagesToBecomeOperational = 1
            });
            var sensors = new[]
            {
                new DistanceMessage
                {
                    Distance = null,
                    SensorAngle = sensorAngle,
                    SensorId = 1
                },
            };
            var target = Vector3.UnitX;
            var result = new MapSimulator(Calculator, sensors, target);
            result.AddRobot(Vector3.Zero);
            result.AddPointObstacle(distance * Vector3.UnitX.Rotate(obstacleAngle));
            return result;
        }

        private ObstacleAvoidanceCalculator Calculator { get; set; }
    }
}