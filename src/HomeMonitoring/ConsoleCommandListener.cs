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
        public void CommandReceived(ZigBeeCommand command)
        {
            if (command is ReportAttributesCommand reportAttributesCommand)
            {
                if (reportAttributesCommand.ClusterId == ZclPressureMeasurementCluster.CLUSTER_ID)
                {
                    var measuredValue = reportAttributesCommand.Reports.FirstOrDefault(
                        o => o.AttributeIdentifier == ZclPressureMeasurementCluster.ATTR_SCALEDVALUE);
                    if (measuredValue != null && measuredValue.AttributeDataType.DataType == DataType.SIGNED_16_BIT_INTEGER)
                    {
                        
                        var value = Convert.ToDouble(measuredValue.AttributeValue);
                        Console.WriteLine($"{DateTime.Now} {value / 100} kPa");
                        return;
                    }
                }
                else if (reportAttributesCommand.ClusterId == ZclRelativeHumidityMeasurementCluster.CLUSTER_ID)
                {
                    var measuredValue = reportAttributesCommand.Reports.FirstOrDefault(
                        o => o.AttributeIdentifier == ZclRelativeHumidityMeasurementCluster.ATTR_MEASUREDVALUE);
                    if (measuredValue != null && measuredValue.AttributeDataType.DataType == DataType.UNSIGNED_16_BIT_INTEGER)
                    {
                        var value = Convert.ToDouble(measuredValue.AttributeValue);
                        Console.WriteLine($"{DateTime.Now} {value / 100} %");
                        return;
                    }
                }
                else if (reportAttributesCommand.ClusterId == ZclTemperatureMeasurementCluster.CLUSTER_ID)
                {
                    var measuredValue = reportAttributesCommand.Reports.FirstOrDefault(
                        o => o.AttributeIdentifier == ZclTemperatureMeasurementCluster.ATTR_MEASUREDVALUE);
                    if (measuredValue != null && measuredValue.AttributeDataType.DataType == DataType.SIGNED_16_BIT_INTEGER)
                    {
                        var value = Convert.ToDouble(measuredValue.AttributeValue);
                        Console.WriteLine($"{DateTime.Now} {value / 100} °C");
                        return;
                    }
                }

                Console.WriteLine(DateTime.Now);
                Console.WriteLine(reportAttributesCommand);
                foreach (var report in reportAttributesCommand.Reports)
                {
                    Console.WriteLine($" Report {report}");
                }
            }
            else
            {
                Console.WriteLine(command);
            }
        }
    }
}
