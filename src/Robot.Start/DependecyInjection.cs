﻿using Iot.Device.MotorHat;
using Robot.MessageBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Robot.Controllers;
using Robot.Controllers.RemoteControl;
using Robot.Drivers.Motors;
using Robot.Drivers.RemoteControl;
using Robot.Reactive;
using Robot.Controllers.Sensor;
using Microsoft.Extensions.Hosting;
using Robot.Drivers.Sonar;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Linq;

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

        public static IServiceCollection AddControlLayer(this IServiceCollection services, HostBuilderContext hostContext)
        {
            // Configure the Differencial Drive Controller
            services.AddHostedService<DifferentialDriveVelocityController>(s =>
            {
                // Create the motor Hat, and the drivers for left and right motors
                var motorHat = new MotorHat();
                var leftMotorDriver = new DCMotorDriver(motorHat.CreateDCMotor(1), s.GetService<ILogger<DCMotorDriver>>());
                var rightMotorDriver = new DCMotorDriver(motorHat.CreateDCMotor(3), s.GetService<ILogger<DCMotorDriver>>());

                // Create the SpeedController that controls left and right motors
                return new DifferentialDriveVelocityController(
                     leftMotorDriver,
                     rightMotorDriver,
                     s.GetService<IMessageBroker>(),
                     s.GetService<ILogger<DifferentialDriveVelocityController>>());
            });

            services.Configure<RemoteControlOptions>(options =>
            {
                options.GamepadKeyThrottle = 1;
                options.GamepadKeyYaw = 0;
            });

            // Configure the Remote Control Controller
            services.AddHostedService<RemoteControlController>(s =>
            {
                var gamepadDriver = new GamepadDriver(s.GetService<ILogger<GamepadDriver>>());

                return new RemoteControlController(
                    gamepadDriver,
                    s.GetService<IMessageBroker>(),
                    s.GetService<IOptions<RemoteControlOptions>>(),
                    s.GetService<ILogger<RemoteControlController>>());
            });

            // Configure the Distance Controller
            services.AddHostedService<DistanceSensorController>(s =>
            {
                var configs = hostContext.Configuration.GetSection("sensors:sonars");
                var mappedConfigs = configs.Get<IEnumerable<HcSr04DistanceSensorDriverSettings>>();
                var sensors = mappedConfigs.Select(x => new HcSr04DistanceSensorDriver(x, s.GetService<ILogger<HcSr04DistanceSensorDriver>>())).ToArray();

                return new DistanceSensorController(sensors,
                    s.GetService<IMessageBroker>(),
                    s.GetService<ILogger<DistanceSensorController>>());
            });

            return services;
        }
    }
}
