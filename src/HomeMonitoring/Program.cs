using Microsoft.Extensions.Configuration;
using OpenTelemetry;
using OpenTelemetry.Trace;
using System;
using System.Threading.Tasks;

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

            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
#if DEBUG
                .AddUserSecrets<Program>()
#endif
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables();

            var configuration = builder.Build();

            var databasePath = configuration.GetValue<string>("databasePath");
            var serialPort = configuration.GetValue<string>("serialPort");

            var sensorScanner = new SensorScanner();
            sensorScanner.Scan(databasePath, serialPort);
            await Task.CompletedTask;
        }
    }
}
