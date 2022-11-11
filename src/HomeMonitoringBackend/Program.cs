using Microsoft.Extensions.Hosting;

namespace HomeMonitoringBackend;

public class Program
{
    public static void Main()
    {
        var host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults()
            //.ConfigureFunctionsWorkerDefaults(builder =>
            //{
            //    builder
            //        .AddApplicationInsights()
            //        .AddApplicationInsightsLogger();
            //})
            .Build();

        host.Run();
    }
}
