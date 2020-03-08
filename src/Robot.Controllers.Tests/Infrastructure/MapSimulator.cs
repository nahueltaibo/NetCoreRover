using System;
using System.Collections.Generic;
using System.Numerics;
using Robot.Controllers.Extensions;
using Robot.Controllers.Motion;
using Robot.MessageBus.Messages;

namespace Robot.Controllers.Tests.Infrastructure
{
    public class MapSimulator
    {
        private readonly ObstacleAvoidanceCalculator _underTest;
        private readonly DistanceMessage[] _sensors;
        private Vector3 _robot;
        private readonly List<Vector3> _obstaclePoints = new List<Vector3>();
        private Vector3 _velocity;
        private float _realSpeed;
        private float _timeInterval;
        private double _effectualAngle;
        private float _velocityModule;
        private Vector3 _target;

        public List<MapCoordinates> Coordinates { get; } = new List<MapCoordinates>();
        public MapSimulator(ObstacleAvoidanceCalculator underTest, DistanceMessage[] sensors, Vector3 target)
        {
            _underTest = underTest;
            _sensors = sensors;
            _realSpeed = 0.05f; // 5 centimeters per second
            _timeInterval = 0.2f; // recalculation every 200 msec
            _effectualAngle = Math.PI / 6; // effective angle is 30 degrees
            _target = target;
        }

        public void AddRobot(Vector3 position)
        {
            _robot = position;
            Coordinates.Add(new MapCoordinates{Position = _robot, Speed = Vector3.Zero});
        }

        public void AddPointObstacle(Vector3 point)
        {
            _obstaclePoints.Add(point);
        }

        public void SetInitialVelocity(float x, float y, float z)
        {
            _velocity = new Vector3(x, y, z);
            _velocityModule = _velocity.Length();
            _underTest.ReceiveRequestedSpeed(new RequestedVelocityMessage
            {
                X = x,
                Y = y,
                Z = z
            });
        }

        public void SimulateSteps(int steps)
        {
            for (int i = 0; i < steps; i++)
            {
                UpdateSensorData();
                var velocity = _underTest.GetResultingVector();
                _velocity = new Vector3(Convert.ToSingle(velocity.X), Convert.ToSingle(velocity.Y), Convert.ToSingle(velocity.Z));
                MakeMove();
            }
        }
        
        private void UpdateSensorData()
        {
            foreach (var sensor in _sensors)
            {
                UpdateSensorData(sensor);
            }
        }

        private void UpdateSensorData(DistanceMessage sensor)
        {
            foreach (var obstaclePoint in _obstaclePoints)
            {
                float? distance = null;
                if (IsVisible(sensor, obstaclePoint))
                {
                    distance = CalculateDistance(obstaclePoint);
                }
                
                _underTest.ReceiveSensorData(new DistanceMessage
                {
                    SensorId = sensor.SensorId,
                    SensorAngle = sensor.SensorAngle,
                    Distance = distance
                });
            }
        }

        private float CalculateDistance(Vector3 obstaclePoint)
        {
            return Vector3.Distance(obstaclePoint, _robot);
        }

        private bool IsVisible(DistanceMessage sensor, Vector3 obstaclePoint)
        {
            if (_velocity.Length() < 1e-5) return false;

            var trajectory = obstaclePoint - _robot;
            var angle = trajectory.AngleTo(_velocity);
            return Math.Abs(sensor.SensorAngle - angle) < _effectualAngle / 2;
        }

        private void MakeMove()
        {
            var movement = _velocity * _timeInterval * _realSpeed;
            _robot += movement;
            Coordinates.Add(new MapCoordinates{Position = _robot, Speed = _velocity});
            _velocity = Vector3.Normalize((_target - _robot)) * _velocityModule;
            
            _underTest.ReceiveRequestedSpeed(new RequestedVelocityMessage
            {
                X = _velocity.X,
                Y = _velocity.Y,
                Z = _velocity.Z
            });
        }
    }
}