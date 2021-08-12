using System;
using ZigBeeNet;
using ZigBeeNet.ZCL.Clusters.General;

namespace HomeMonitoring
{
    public class ConsoleCommandListener : IZigBeeCommandListener
    {
        public void CommandReceived(ZigBeeCommand command)
        {
            Console.WriteLine(command);
            if (command is ReportAttributesCommand reportAttributesCommand)
            {
                foreach (var report in reportAttributesCommand.Reports)
                {
                    Console.WriteLine($" Report {report}");
                }
            }
        }
    }
}
