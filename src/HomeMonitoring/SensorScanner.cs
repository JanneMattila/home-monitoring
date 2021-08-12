using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using ZigbeeNet.Hardware.ConBee;
using ZigBeeNet;
using ZigBeeNet.App.Basic;
using ZigBeeNet.App.Discovery;
using ZigBeeNet.App.IasClient;
using ZigBeeNet.DataStore.Json;
using ZigBeeNet.Tranport.SerialPort;
using ZigBeeNet.Transaction;
using ZigBeeNet.ZCL.Clusters;

namespace HomeMonitoring
{
    public class SensorScanner
    {
        private readonly ActivitySource _source = new ActivitySource("SensorScanner");
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public SensorScanner()
        {
            Console.CancelKeyPress += (sender, a) =>
            {
                a.Cancel = true;
                Console.WriteLine("Closing down...");
                _cancellationTokenSource.Cancel();
            };
        }

        public void Scan(string navigateUri)
        {
            var activity = _source.StartActivity("ScanAsync");

            var folderPath = @"\temp\zigbee";

            var zigbeePort = new ZigBeeSerialPort("COM3");
            var dongle = new ZigbeeDongleConBee(zigbeePort);
            var networkManager = new ZigBeeNetworkManager(dongle);
            networkManager.AddNetworkNodeListener(new ConsoleNetworkNodeListener());
            networkManager.AddCommandListener(new ZigBeeTransaction(networkManager));
            networkManager.AddCommandListener(new ConsoleCommandListener());

            networkManager.AddSupportedClientCluster(ZclOnOffCluster.CLUSTER_ID);
            networkManager.AddSupportedClientCluster(ZclMultistateOutputBasicCluster.CLUSTER_ID);
            networkManager.AddSupportedClientCluster(ZclColorControlCluster.CLUSTER_ID);
            networkManager.AddSupportedClientCluster(ZclMeteringCluster.CLUSTER_ID);
            networkManager.AddSupportedClientCluster(ZclPressureMeasurementCluster.CLUSTER_ID);

            networkManager.AddExtension(new ZigBeeBasicServerExtension());
            networkManager.AddExtension(new ZigBeeIasCieExtension());

            var dataStore = new JsonNetworkDataStore(folderPath);
            networkManager.SetNetworkDataStore(dataStore);

            var discoveryExtension = new ZigBeeDiscoveryExtension();
            discoveryExtension.SetUpdatePeriod(60);
            networkManager.AddExtension(discoveryExtension);

            networkManager.Initialize();

            var startupSucceded = networkManager.Startup(false);
            if (startupSucceded != ZigBeeStatus.SUCCESS)
            {
                return;
            }

            Console.WriteLine($"PAN ID           = {networkManager.ZigBeePanId}");
            Console.WriteLine($"Extended PAN ID  = {networkManager.ZigBeeExtendedPanId}");
            Console.WriteLine($"Channel          = {networkManager.ZigbeeChannel}");

            var coordinator = networkManager.GetNode(0);

            // ZclTemperatureMeasurementCluster.CLUSTER_ID
            //{ 0x0402, new ZclClusterType(0x0402, "Temperature Measurement", ClusterType.TEMPERATURE_MEASUREMENT, (endpoint) => new ZclTemperatureMeasurementCluster(endpoint)) },
            //{ 0x0403, new ZclClusterType(0x0403, "Pressure Measurement", ClusterType.PRESSURE_MEASUREMENT, (endpoint) => new ZclPressureMeasurementCluster(endpoint)) },
            //{ 0x0405, new ZclClusterType(0x0405, "Relative Humidity Measurement", ClusterType.RELATIVE_HUMIDITY_MEASUREMENT, (endpoint) => new ZclRelativeHumidityMeasurementCluster(endpoint)) },

            coordinator.PermitJoin(true);
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                var nodes = networkManager.Nodes;
                foreach (var node in nodes)
                {
                    Console.WriteLine($"{node.LogicalType} {node.NetworkAddress} {node.LastUpdateTime}");
                    if (node.Endpoints.Any())
                    {
                        var attributes = node.Endpoints.FirstOrDefault().Value
                           .GetInputCluster(ZclBasicCluster.CLUSTER_ID)
                           .GetAttributes();
                        foreach (var attribute in attributes)
                        {
                            if (attribute.LastReportTime != DateTime.MinValue)
                            {
                                Console.WriteLine($" - {attribute.Name}: {attribute.LastValue} {attribute.LastReportTime}");
                            }
                        }
                        var attributes2 = node.Endpoints.FirstOrDefault().Value
                           .GetInputCluster(ZclTemperatureMeasurementCluster.CLUSTER_ID)
                           .GetAttributes();
                        foreach (var attribute in attributes2)
                        {
                            if (attribute.LastReportTime != DateTime.MinValue)
                            {
                                Console.WriteLine($" - {attribute.Name}: {attribute.LastValue} {attribute.LastReportTime}");
                            }
                        }
                        var attributes3 = node.Endpoints.FirstOrDefault().Value
                           .GetInputCluster(ZclRelativeHumidityMeasurementCluster.CLUSTER_ID)
                           .GetAttributes();
                        foreach (var attribute in attributes3)
                        {
                            if (attribute.LastReportTime != DateTime.MinValue)
                            {
                                Console.WriteLine($" - {attribute.Name}: {attribute.LastValue} {attribute.LastReportTime}");
                            }
                        }
                    }
                }

                Thread.Sleep(5_000);
                Console.WriteLine(new string('*', 30));
                Console.ReadLine();
            }

            networkManager.Shutdown();
        }
    }
}
