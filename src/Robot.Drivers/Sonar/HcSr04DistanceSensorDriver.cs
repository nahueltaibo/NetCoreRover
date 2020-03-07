﻿using Iot.Device.Hcsr04;
using Microsoft.Extensions.Logging;
using System;
using System.Device.Gpio;
using System.Threading;
using System.Threading.Tasks;

namespace Robot.Drivers.Sonar
{
    public class HcSr04DistanceSensorDriver : IDistanceSensor
    {
        private readonly Hcsr04 Sensor;

        public int Id { get; }
        public double Angle { get; }
        private CancellationTokenSource MeasuringCancellationToken { get; set; }
        private HcSr04DistanceSensorDriverSettings Settings { get; }
        private ILogger<HcSr04DistanceSensorDriver> Logger { get; }

        public event EventHandler<SonarDistanceEventArgs> SonarDistanceChanged;

        public HcSr04DistanceSensorDriver(HcSr04DistanceSensorDriverSettings settings, ILogger<HcSr04DistanceSensorDriver> logger)
        {
            Id = settings.SensorId;
            Angle = settings.Angle;
            Sensor = new Hcsr04(settings.TriggerPin, settings.EchoPin, PinNumberingScheme.Logical);
            MeasuringCancellationToken = new CancellationTokenSource();
            Task.Run(MeasurementCycle);
            Settings = settings;
            Logger = logger;
        }

        private async void MeasurementCycle()
        {
            var token = MeasuringCancellationToken.Token;
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(Settings.MeasuringInterval);
                try
                {
                    if (SonarDistanceChanged != null)
                    {
                        var measurement = Sensor.Distance / 100; // Convert from cm to m
                        SonarDistanceChanged.Invoke(this, new SonarDistanceEventArgs
                        {
                            SonarId = Id,
                            Angle = Angle,
                            Distance = measurement
                        });
                    }                    
                    
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "Sonar {0} failed to raise a SonarDistanceChanged event", Id);
                }
            }
        }

        ~HcSr04DistanceSensorDriver()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                Sensor.Dispose();
                MeasuringCancellationToken.Cancel();
            }
        }
    }
}
