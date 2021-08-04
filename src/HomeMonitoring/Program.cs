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
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            var configuration = builder.Build();

            var navigateUri = configuration.GetValue<string>("navigateUri");

            var sensorScanner = new SensorScanner();
            await sensorScanner.ScanAsync(navigateUri);
        }
    }
}
