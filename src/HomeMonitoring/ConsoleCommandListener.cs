using HomeMonitoring.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using ZigBeeNet;
using ZigBeeNet.ZCL.Clusters;
using ZigBeeNet.ZCL.Clusters.General;
using ZigBeeNet.ZCL.Protocol;

namespace HomeMonitoring
{
    public class ConsoleCommandListener : IZigBeeCommandListener
    {
        public DateTime _started = DateTime.Now;
        public readonly string _sensorDataPath;

        public ConsoleCommandListener(string sensorDataPath)
        {
            _sensorDataPath = sensorDataPath;
        }

        public bool ProcessMeasurement(
            ReportAttributesCommand reportAttributesCommand,
            string measurementName,
            ushort attributeIdentifier,
            double modifier,
            string unit)
        {
            var measuredValue = reportAttributesCommand.Reports.FirstOrDefault(o => o.AttributeIdentifier == attributeIdentifier);
            if (measuredValue != null)
            {
                var now = DateTime.Now;
                var value = Convert.ToDouble(measuredValue.AttributeValue);
                Console.WriteLine($"{DateTime.Now} {(now - _started).TotalSeconds:F0} {value / modifier} {unit}");
                _started = now;

                var sensorData = new SensorData()
                {
                    ID = reportAttributesCommand.SourceAddress.Address.ToString(),
                    Name = measurementName,
                    Measurement = measurementName,
                    Time = DateTimeOffset.Now,
                    Value = value / modifier,
                    Unit = unit
                };
                var json = JsonSerializer.Serialize(sensorData);
                File.WriteAllText($"{_sensorDataPath}{Path.DirectorySeparatorChar}{sensorData.ID}{now:yyyy-MM-dd-HH-mm-ss-ffff}.json", json);
                return true;
            }
            return false;
        }
        public void CommandReceived(ZigBeeCommand command)
        {
            if (command is ReportAttributesCommand reportAttributesCommand)
            {
                var processed = false;
                if (reportAttributesCommand.ClusterId == ZclPressureMeasurementCluster.CLUSTER_ID)
                {
                    processed = ProcessMeasurement(reportAttributesCommand,
                        ZclPressureMeasurementCluster.CLUSTER_NAME,
                        ZclPressureMeasurementCluster.ATTR_SCALEDVALUE, 100, "kPa");
                }
                else if (reportAttributesCommand.ClusterId == ZclRelativeHumidityMeasurementCluster.CLUSTER_ID)
                {
                    processed = ProcessMeasurement(reportAttributesCommand,
                        ZclRelativeHumidityMeasurementCluster.CLUSTER_NAME,
                        ZclRelativeHumidityMeasurementCluster.ATTR_MEASUREDVALUE, 100, "%");
                }
                else if (reportAttributesCommand.ClusterId == ZclTemperatureMeasurementCluster.CLUSTER_ID)
                {
                    processed = ProcessMeasurement(reportAttributesCommand,
                        ZclTemperatureMeasurementCluster.CLUSTER_NAME,
                        ZclTemperatureMeasurementCluster.ATTR_MEASUREDVALUE, 100, "°C");
                }

                if (!processed)
                {
                    Console.WriteLine(DateTime.Now);
                    Console.WriteLine(reportAttributesCommand);
                    foreach (var report in reportAttributesCommand.Reports)
                    {
                        Console.WriteLine($" Report {report}");
                    }
                }
            }
            else
            {
                Console.WriteLine(command);
            }
        }
    }
}
