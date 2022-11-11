using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace HomeMonitoringBackend;

public static class EventHubProcessorFunction
{
    [Function("EventHubProcessorFunction")]
    public static async Task Run(
        [EventHubTrigger("history", Connection = "EventHub")] string[] events,
        Binder binder,
        ILogger log)
    {
        var json = $"[{string.Join(',', events)}]";
        var filename = DateTime.Now.ToString("yyyy/MM/dd/HHmmss-fffff", CultureInfo.InvariantCulture);
        var attributes = new Attribute[]
        {
            new BlobAttribute($"history/{filename}.json", FileAccess.Write),
            new StorageAccountAttribute("HistoryStorage")
        };

        using var writer = await binder.BindAsync<TextWriter>(attributes);
        writer.Write(json);
    }
}
