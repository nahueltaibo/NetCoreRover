using System;
using System.Device.Gpio;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Robot.Drivers.Switches
{
    public class SwitchSensorDriver : ISwitchSensor
    {  
        public int Id { get; }
        public int PinId { get; }
        public bool IsOn { get; }
        private CancellationTokenSource ScanCancellationToken { get; set; }
        private SwitchSensorDriverSettings Settings { get; }
        private ILogger<SwitchSensorDriver> Logger { get; }

        public event EventHandler<SwitchSensorEventArgs> SwitchPositionChanged;

        public SwitchSensorDriver(SwitchSensorDriverSettings settings, ILogger<SwitchSensorDriver> logger)
        {
            Id = settings.SensorId;
            PinId = settings.SwitchPin;
                       
            ScanCancellationToken = new CancellationTokenSource();
            Task.Run(() => ScanningCycle());
            Settings = settings;
            Logger = logger;
        }

        private async void ScanningCycle()
        {
            var token = ScanCancellationToken.Token;
            while (!token.IsCancellationRequested)
            {
                var interval = Task.Delay(Settings.MeasuringInterval);
                DetectingToggleSwitch();
                await Task.WhenAll(interval);
            }
        }

        private void DetectingToggleSwitch()
        {
            Task.Run(() =>
            {
                using (var controller = new GpioController())
                {
                    controller.OpenPin(PinId, PinMode.Input);

                    if (SwitchPositionChanged != null)
                    {
                        // Check input value each time we loop
                        var switchPosition = (controller.Read(PinId) == PinValue.High);

                        SwitchPositionChanged.Invoke(this, new SwitchSensorEventArgs
                        {
                            SwitchId = Id,
                            IsOn = switchPosition
                        });
                    }
                }
            });
        }

        ~SwitchSensorDriver()
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
                ScanCancellationToken.Cancel();
            }
        }
    }
}