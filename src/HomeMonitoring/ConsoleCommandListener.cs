using System;
using System.Linq;
using ZigBeeNet;
using ZigBeeNet.ZCL.Clusters;
using ZigBeeNet.ZCL.Clusters.General;
using ZigBeeNet.ZCL.Protocol;

namespace HomeMonitoring
{
    public class ConsoleCommandListener : IZigBeeCommandListener
    {
        public DateTime _started = DateTime.Now;

        public bool ProcessMeasurement(
            ReportAttributesCommand reportAttributesCommand,
            ushort attributeIdentifier,
            double modifier,
            string unit)
        {
            var measuredValue = reportAttributesCommand.Reports.FirstOrDefault(o => o.AttributeIdentifier == attributeIdentifier);
            if (measuredValue != null)
            {

                var value = Convert.ToDouble(measuredValue.AttributeValue);
                Console.WriteLine($"{DateTime.Now} {(DateTime.Now - _started).TotalSeconds:F0} {value / modifier} {unit}");
                _started = DateTime.Now;
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
                    processed = ProcessMeasurement(reportAttributesCommand, ZclPressureMeasurementCluster.ATTR_SCALEDVALUE, 100, "kPa");
                }
                else if (reportAttributesCommand.ClusterId == ZclRelativeHumidityMeasurementCluster.CLUSTER_ID)
                {
                    processed = ProcessMeasurement(reportAttributesCommand, ZclRelativeHumidityMeasurementCluster.ATTR_MEASUREDVALUE, 100, "%");
                }
                else if (reportAttributesCommand.ClusterId == ZclTemperatureMeasurementCluster.CLUSTER_ID)
                {
                    processed = ProcessMeasurement(reportAttributesCommand, ZclTemperatureMeasurementCluster.ATTR_MEASUREDVALUE, 100, "°C");
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
