using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Threading;
using Robot.Controllers.Extensions;
using Robot.MessageBus.Messages;

namespace Robot.Controllers.Motion
{
    /// <summary>
    /// Class that supports mathematical operations to avoid detected obstacles
    /// </summary>
    /// <remarks>This class is not thread-safe</remarks>
    public class ObstacleAvoidanceCalculator
    {
        private const float MinimalLengthThreshold = 1e-3f;
        private const float MinimalAngleThreshold = (float) Math.PI / 180;
        private readonly ObstacleAvoidanceCalculatorSettings _settings;
        private readonly ConcurrentDictionary<int, SensorData> _sensors = new ConcurrentDictionary<int, SensorData>();
        private readonly ConcurrentDictionary<int, float?> _distances = new ConcurrentDictionary<int, float?>();
        private Vector3 _requestedSpeed;
        private int _receivedSensorData;

        /// <summary>
        /// Shows if the calculator gathered enough data from sensors to work
        /// </summary>
        /// <remarks>It needs at least several messages from each sensor to become operational.</remarks>
        public bool IsReady { get; private set; }

        /// <summary>
        /// Internal flag representing that speed vector can be processed
        /// </summary>
        private bool CanProcess { get; set; }

        public ObstacleAvoidanceCalculator(ObstacleAvoidanceCalculatorSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            ValidateSettings();
        }

        private void ValidateSettings()
        {
            if (_settings.DistanceThreshold <= 0f)
            {
                throw new ArgumentException("DistanceThreshold is expected to be positive");
            }

            if (_settings.MinimalDistancePercent <= 0f || _settings.MinimalDistancePercent > 1.0f)
            {
                throw new ArgumentException("MinimalDistancePercent is expected to be in a range of (0..1]");
            }
        }

        public void ReceiveSensorData(DistanceMessage message)
        {
            if (!IsReady)
            {
                _sensors.TryAdd(message.SensorId, new SensorData
                {
                    SensorAngle = Convert.ToSingle(message.SensorAngle)
                });
                var total = Interlocked.Increment(ref _receivedSensorData);
                IsReady = total >= _settings.MessagesToBecomeOperational;
            }

            float? distance = message.Distance == null ? (float?) null : Convert.ToSingle(message.Distance.Value);
            _distances.AddOrUpdate(message.SensorId, distance, (key, value) => distance);
        }

        public void ReceiveRequestedSpeed(RequestedVelocityMessage message)
        {
            _requestedSpeed = new Vector3(Convert.ToSingle(message.X ?? 0f),
                Convert.ToSingle(message.Y ?? 0f), Convert.ToSingle(message.Z ?? 0f));
            CanProcess = _requestedSpeed.Length() > MinimalLengthThreshold;
        }

        public AppliedVelocityMessage GetResultingVector()
        {
            if (!CanProcess)
            {
                return _requestedSpeed.ToAppliedVelocityMessage();
            }

            var fieldItems = _distances.Count;
            var fieldComponents = new Vector3[fieldItems];
            using (var enumerator = _distances.GetEnumerator())
            {
                for (int i = 0; i < fieldItems; i++)
                {
                    enumerator.MoveNext();
                    fieldComponents[i] = CalculateFieldComponent(enumerator.Current);
                }

                var resultingFieldForce = ApplyFieldComponents(fieldComponents);
                return resultingFieldForce.ToAppliedVelocityMessage();
            }
        }

        private Vector3 ApplyFieldComponents(IEnumerable<Vector3> fieldComponents)
        {
            Debug.Assert(CanProcess,
                $"{nameof(_requestedSpeed)} should be larger that a threshold ({MinimalLengthThreshold})");

            var resultingVector = Vector3.Zero;
            foreach (var component in fieldComponents)
            {
                resultingVector += component;
            }

            var angleBetweenVictors = _requestedSpeed.AngleTo(resultingVector);
            var rotationAngle = CalculateRotationAngle(angleBetweenVictors, resultingVector.Length());
            resultingVector = _requestedSpeed.Rotate(rotationAngle);
            return resultingVector;
        }

        private double CalculateRotationAngle(double angle, double fieldValue)
        {
            while (Math.Abs(angle) > Math.PI)
            {
                angle += angle > 0 ? -2 * Math.PI : 2 * Math.PI;
            }

            fieldValue = Math.Min(1, fieldValue);

            // We're to the right to a Gravity center
            if (angle > MinimalAngleThreshold)
            {
                angle -= Math.PI / 2;
                return angle * fieldValue;
            }

            // We're to the left to a Gravity center
            if (angle < -MinimalAngleThreshold)
            {
                angle += Math.PI / 2;
                return angle * fieldValue;
            }

            // The gravity center is right in front of us
            // Doing preemptive rotation to the right
            return 0; //Math.PI / 18;
        }

        private Vector3 CalculateFieldComponent(in KeyValuePair<int, float?> distance)
        {
            if (!distance.Value.HasValue)
            {
                return Vector3.Zero;
            }

            var theta = _sensors[distance.Key].SensorAngle +
                        Math.PI; // Force field vector should be opposed to the sensor direction
            var normalizedSource = Vector3.Normalize(_requestedSpeed);
            var vector = normalizedSource.Rotate(theta);
            var coefficient = CalculateCoefficient(distance.Value.Value);
            return coefficient * vector;
        }

        private float CalculateCoefficient(float distance)
        {
            var minDistance = _settings.MinimalDistancePercent * _settings.DistanceThreshold;
            if (distance > 0f && distance <= _settings.DistanceThreshold)
            {
                if (distance < minDistance)
                {
                    return 1;
                }

                return (_settings.DistanceThreshold - distance - minDistance) / ( _settings.DistanceThreshold - minDistance);
            }

            return 0f;
        }

        private class SensorData
        {
            public float SensorAngle { get; set; }
        }
    }
}