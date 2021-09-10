using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace HomeMonitoringBackend
{
    public static class EventHubProcessorFunction
    {
        [FunctionName("EventHubProcessorFunction")]
        public static async Task Run(
            [EventHubTrigger("history", Connection = "EventHub")] EventData[] events,
            Binder binder,
            ILogger log)
        {
            var sb = new StringBuilder();
            sb.Append("[");
            var messages = new List<string>();
            foreach (var eventData in events)
            {
                try
                {
                    var messageBody = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                    messages.Add(messageBody);
                }
                catch (Exception ex)
                {
                    log.LogError("Failed to process message from event hub", ex);
                }
            }

            sb.Append(string.Join(',', messages));
            sb.Append("]");

            var filename = DateTime.Now.ToString("yyyy/MM/dd/HHmmss-fffff", CultureInfo.InvariantCulture);
            var attributes = new Attribute[]
            {
                new BlobAttribute($"history/{filename}.json", FileAccess.Write),
                new StorageAccountAttribute("HistoryStorage")
            };

            using var writer = await binder.BindAsync<TextWriter>(attributes);
            writer.Write(sb.ToString());
        }
    }
}
