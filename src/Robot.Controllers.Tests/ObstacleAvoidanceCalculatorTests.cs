using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using Robot.Controllers.Extensions;
using Robot.Controllers.Motion;
using Robot.Controllers.Tests.Infrastructure;
using Robot.MessageBus.Messages;

namespace Robot.Controllers.Tests
{
    public class ObstacleAvoidanceCalculatorTests
    {
        private readonly List<DistanceMessage> _sensors = new List<DistanceMessage>();

        [SetUp]
        public void Setup()
        {
            _sensors.Clear();
        }

        [Test]
        public void WhenTheObstacleIsClockwiseRotatesCounter()
        {
            var underTest = CreateCalculator();
            var sensorData = new DistanceMessage
            {
                SensorId = _sensors[2].SensorId,
                SensorAngle = _sensors[2].SensorAngle,
                Distance = 0.1
            };
            underTest.ReceiveSensorData(sensorData);
            underTest.ReceiveRequestedSpeed(new RequestedVelocityMessage
            {
                X = 1
            });
            var result = underTest.GetResultingVector();
            var vector = new Vector3(Convert.ToSingle(result.X ?? 0), Convert.ToSingle(result.Y ?? 0),
                Convert.ToSingle(result.Z ?? 0));
            Assert.IsTrue(Vector3.UnitX.AngleTo(vector) > 0);
        }

        [Test]
        public void WhenTheObstacleIsCounterclockwiseRotatesClockwise()
        {
            var underTest = CreateCalculator();
            var sensorData = new DistanceMessage
            {
                SensorId = _sensors[0].SensorId,
                SensorAngle = _sensors[0].SensorAngle,
                Distance = 0.1
            };
            underTest.ReceiveSensorData(sensorData);
            underTest.ReceiveRequestedSpeed(new RequestedVelocityMessage
            {
                X = 1
            });
            var result = underTest.GetResultingVector();
            var vector = new Vector3(Convert.ToSingle(result.X ?? 0), Convert.ToSingle(result.Y ?? 0),
                Convert.ToSingle(result.Z ?? 0));
            Assert.IsTrue(Vector3.UnitX.AngleTo(vector) < 0);
        }

        [Test]
        public void WhenTheObstacleIsInFrontRotatesCounterclockwise()
        {
            var underTest = CreateCalculator();
            var sensorData = new DistanceMessage
            {
                SensorId = _sensors[1].SensorId,
                SensorAngle = _sensors[1].SensorAngle,
                Distance = 0.1
            };
            underTest.ReceiveSensorData(sensorData);
            underTest.ReceiveRequestedSpeed(new RequestedVelocityMessage
            {
                X = 1
            });
            var result = underTest.GetResultingVector();
            var vector = new Vector3(Convert.ToSingle(result.X ?? 0), Convert.ToSingle(result.Y ?? 0),
                Convert.ToSingle(result.Z ?? 0));
            Assert.IsTrue(Vector3.UnitX.AngleTo(vector) < 0);
        }

        [Test]
        public void WhenTheObstacleIsClose_Turns90degrees()
        {
            var underTest = CreateCalculator();
            var sensorData = new DistanceMessage
            {
                SensorId = _sensors[1].SensorId,
                SensorAngle = _sensors[1].SensorAngle,
                Distance = 0.05
            };
            underTest.ReceiveSensorData(sensorData);
            underTest.ReceiveRequestedSpeed(new RequestedVelocityMessage
            {
                X = 1
            });
            var result = underTest.GetResultingVector();
            var vector = new Vector3(Convert.ToSingle(result.X ?? 0), Convert.ToSingle(result.Y ?? 0),
                Convert.ToSingle(result.Z ?? 0));
            Assert.AreEqual(-Math.PI / 2, Vector3.UnitX.AngleTo(vector), 1e-5);
        }

        /// <summary>
        /// Tests if the Calculator guides a robot around a point particle right in front of it.
        /// Robot should pass the point particle in 100 steps and without hitting a point
        /// </summary>
        [TestCase(0.1f)]
        [TestCase(0.2f)]
        [TestCase(0.3f)]
        [TestCase(0.4f)]
        [TestCase(0.5f)]
        public void AvoidsSinglePoint(float distance)
        {
            var target = new Vector3(distance * 2f, 0, 0);
            var obstacle = new Vector3(distance, 0, 0);

            var underTest = CreateCalculator();
            var map = new MapSimulator(underTest, _sensors.ToArray(), target);
            map.AddRobot(Vector3.Zero);

            map.AddPointObstacle(obstacle);
            map.SetInitialVelocity(1.0f, 0f, 0f);
            map.SimulateSteps(100);

            Assert.AreEqual(101, map.Coordinates.Count); // 100 + initial position
            
            var lastPoint = map.Coordinates.Last();
            Assert.IsTrue(Vector3.Distance(lastPoint.Position, target) < 1e-2f);
        }

        private ObstacleAvoidanceCalculator CreateCalculator()
        {
            var messagesToInitialize = 3;
            Settings = new ObstacleAvoidanceCalculatorSettings
            {
                MessagesToBecomeOperational = messagesToInitialize
            };
            var result = new ObstacleAvoidanceCalculator(Settings);

            var currentAngle = Math.PI / 6;
            var angleStep = currentAngle;
            for (int i = 0; i < messagesToInitialize; i++)
            {
                var message = new DistanceMessage
                {
                    SensorId = i,
                    SensorAngle = currentAngle,
                };
                _sensors.Add(message);
                result.ReceiveSensorData(message);
                currentAngle -= angleStep;
            }

            Assert.IsTrue(result.IsReady);
            return result;
        }

        private ObstacleAvoidanceCalculatorSettings Settings { get; set; }
    }
}