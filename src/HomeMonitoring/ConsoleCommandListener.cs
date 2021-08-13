using System;
using ZigBeeNet;
using ZigBeeNet.ZCL.Clusters.General;

namespace HomeMonitoring
{
    public class ConsoleCommandListener : IZigBeeCommandListener
    {
        public void CommandReceived(ZigBeeCommand command)
        {
            if (command is ReportAttributesCommand reportAttributesCommand)
            {
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
