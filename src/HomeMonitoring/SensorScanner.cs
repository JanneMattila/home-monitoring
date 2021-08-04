using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HomeMonitoring
{
    public class SensorScanner
    {
        private readonly ActivitySource _source = new("SensorScanner");

        public async Task ScanAsync(string navigateUri)
        {
            using var activity = _source.StartActivity("ScanAsync");

            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
    }
}
