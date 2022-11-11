using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace HomeMonitoringBackend;

public class EventHubProcessorFunction
{
    private readonly ILogger _logger;

    public EventHubProcessorFunction(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<EventHubProcessorFunction>();
    }

    [Function("EventHubProcessorFunction")]
    public async Task Run([EventHubTrigger("history", Connection = "EventHub")] string[] events)
    {
        var json = $"[{string.Join(',', events)}]";
        var filename = DateTime.Now.ToString("yyyy/MM/dd/HHmmss-fffff", CultureInfo.InvariantCulture);

        var connectionString = Environment.GetEnvironmentVariable("HistoryStorage");
        var blobClient = new BlobClient(connectionString, "history", filename + ".json");
        await blobClient.UploadAsync(BinaryData.FromString(json));
    }
}
