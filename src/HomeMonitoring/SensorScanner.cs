using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZigBeeNet.Transport;
using ZigBeeNet;
using ZigBeeNet.Tranport.SerialPort;
using ZigbeeNet.Hardware.ConBee;
using ZigBeeNet.ZCL.Clusters;
using ZigBeeNet.Database;
using ZigBeeNet.DataStore.Json;

namespace HomeMonitoring
{
    public class SensorScanner
    {
        private readonly ActivitySource _source = new("SensorScanner");

        public async Task ScanAsync(string navigateUri)
        {
            using var activity = _source.StartActivity("ScanAsync");

            var folderPath = @"\temp\zigbee";

            var zigbeePort = new ZigBeeSerialPort("COM3");
            var dongle = new ZigbeeDongleConBee(zigbeePort);
            var networkManager = new ZigBeeNetworkManager(dongle);

            var dataStore = new JsonNetworkDataStore(folderPath);
            networkManager.SetNetworkDataStore(dataStore);

            networkManager.Initialize();

            var startupSucceded = networkManager.Startup(false);
            if (startupSucceded != ZigBeeStatus.SUCCESS)
            {
                return;
            }

            while (true)
            {
                var nodes = networkManager.Nodes;
                foreach (var node in nodes)
                {
                    Console.WriteLine(node.IeeeAddress);
                    if (node.Endpoints.Any())
                    {
                        var attributes = node.Endpoints.FirstOrDefault().Value
                           .GetInputCluster(ZclBasicCluster.CLUSTER_ID)
                           .GetAttributes();
                        foreach (var attribute in attributes)
                        {
                            Console.WriteLine(attribute.Name);
                        }
                    }
                }
                await Task.Delay(TimeSpan.FromSeconds(5));
                Console.WriteLine("***");
            }
        }
    }
}
