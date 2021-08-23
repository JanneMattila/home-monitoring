using System;
using System.Text.Json.Serialization;

namespace HomeMonitoring.Interfaces
{
    public class SensorData
    {
        [JsonPropertyName("id")]
        public string ID {  get; set; }

        [JsonPropertyName("name")]
        public string Name {  get; set; }

        [JsonPropertyName("time")]
        public DateTimeOffset Time { get; set; }

        [JsonPropertyName("measurement")]
        public string Measurement { get; set; }

        [JsonPropertyName("value")]
        public double Value { get; set; }

        [JsonPropertyName("unit")]
        public string Unit { get; set; }
    }
}
