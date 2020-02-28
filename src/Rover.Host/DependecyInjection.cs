using Iot.Device.MotorHat;
using MessageBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Robot.Controllers;
using Robot.Controllers.RemoteControl;
using Robot.Drivers;
using Robot.Drivers.RemoteControl;
using Robot.Reactive;

namespace Robot.Host
{
    public static class DependecyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddLogging();
            services.AddSingleton<IMessageBroker, MesageBroker>();

            return services;
        }

        public static IServiceCollection AddBehavioralLayer(this IServiceCollection services)
        {

            return services;
        }

        public static IServiceCollection AddReactiveLayer(this IServiceCollection services)
        {
            services.AddHostedService<RemoteControlAgent>();

            return services;
        }

        public static IServiceCollection AddControlLayer(this IServiceCollection services)
        {
            // Configure the Differencial Drive Controller
            services.AddSingleton<ISpeedController, DifferentialDriveSpeedController>(s =>
            {
                // Create the motor Hat, and the drivers for left and right motors
                var motorHat = new MotorHat();
                var leftMotorDriver = new DCMotorDriver(motorHat.CreateDCMotor(1), s.GetService<ILogger<DCMotorDriver>>());
                var rightMotorDriver = new DCMotorDriver(motorHat.CreateDCMotor(3), s.GetService<ILogger<DCMotorDriver>>());

                // Create the SpeedController that controls them
                return new DifferentialDriveSpeedController(
                     leftMotorDriver,
                     rightMotorDriver,
                     s.GetService<IMessageBroker>(),
                     s.GetService<ILogger<DifferentialDriveSpeedController>>());
            });

            services.Configure<RCOptions>(options =>
            {
                options.GamepadTranslationAxisKey = 0;
                options.GamepadRotationAxisKey = 1;
            });

            // Configure the Remote Control Controller
            services.AddSingleton<IRCController, RCController>(s =>
            {
                var gamepadDriver = new GamepadDriver(s.GetService<ILogger<GamepadDriver>>());

                return new RCController(
                    gamepadDriver,
                    s.GetService<IMessageBroker>(),
                    s.GetService<IOptions<RCOptions>>(),
                    s.GetService<ILogger<RCController>>());
            });

            return services;
        }
    }
}
