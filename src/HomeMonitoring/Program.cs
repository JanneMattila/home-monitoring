using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using OpenTelemetry;
using OpenTelemetry.Trace;
using System;
using System.Threading.Tasks;
using ZigBeeNet.Util;

namespace HomeMonitoring
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Home Monitoring started");
            using var openTelemetry = Sdk.CreateTracerProviderBuilder()
                .AddSource("HomeMonitoring")
                .AddConsoleExporter()
                .Build();

            // Set logging for ZigBeeNet
            ILoggerFactory _factory = LoggerFactory.Create(builder =>
            {
                builder
                    .SetMinimumLevel(LogLevel.Debug)
                    .AddDebug()
                    .AddSimpleConsole(c =>
                    {
                        c.ColorBehavior = LoggerColorBehavior.Enabled;
                        c.SingleLine = true;
                        c.TimestampFormat = "[HH:mm:ss] ";
                    });
            });
            LogManager.SetFactory(_factory);

            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
#if DEBUG
                .AddUserSecrets<Program>()
#endif
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables();

            var configuration = builder.Build();

            var databasePath = configuration.GetValue<string>("databasePath");
            var sensorDataPath = configuration.GetValue<string>("sensorDataPath");
            var serialPort = configuration.GetValue<string>("serialPort");

            var sensorScanner = new SensorScanner();
            sensorScanner.Scan(databasePath, sensorDataPath, serialPort);
            await Task.CompletedTask;
        }
    }
}
