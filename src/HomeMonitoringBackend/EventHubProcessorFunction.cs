using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeMonitoringBackend
{
    public static class EventHubProcessorFunction
    {
        [FunctionName("EventHubProcessorFunction")]
        [return: Blob("history/{DateTime}", Connection = "Storage")]
        public static string Run(
            [EventHubTrigger("history", Connection = "EventHub")] EventData[] events,
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
            return sb.ToString();
        }
    }
}
