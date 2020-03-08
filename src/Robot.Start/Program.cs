using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace Robot.Start
{
    static class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // If the app is called with "debug" as first argument, it will wait in the following loop for
                // you to attach to the process to debug. Add a breakpoint within the while, and once you
                // attach you connect the debugger to the process, manually move the execution outside the while body
                // so the application can run with the debugger attached from the beginning
                if (args.Length > 0 && args[0].ToLower().Contains("debug"))
                {
                    Console.WriteLine("Waiting for debugger to attach...");
                    while (!Debugger.IsAttached)
                    {
                        Thread.Sleep(100);
                    }
                    Console.WriteLine("Debugger attached.");
                }

                ConfigureLogging();

                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.ReadLine();
            }
        }

        private static void ConfigureLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code, outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}")
                .WriteTo.File("logs\\rover.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.SetBasePath(Directory.GetCurrentDirectory());
                    configApp.AddJsonFile("appsettings.json");
                    configApp.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddInfrastructure();

                    services.AddControlLayer(hostContext);

                    services.AddReactiveLayer();

                    services.AddBehavioralLayer();
                })
                .UseSerilog();
        }
    }
}
