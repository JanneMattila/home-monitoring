using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Core;
using InfluxDB.Client.Writes;

Console.WriteLine("InfluxDB demo");
var token = Environment.GetEnvironmentVariable("INFLUX_TOKEN")!;
const string bucket = "demo";
const string org = "admin";

using var client = InfluxDBClientFactory.Create("http://localhost:8086", token);

//var deleteApi = client.GetDeleteApi();
//await deleteApi.Delete(new DateTime(2000, 1, 1), new DateTime(2100, 1, 1), "_measurement=\"sensor2\"", bucket, org);

var writeApi = client.GetWriteApiAsync();
var random = new Random();

while (true)
{
    var now = DateTime.UtcNow;
    var temperature1 = random.Next(20, 30);
    var point1 = PointData
        .Measurement("sensor1")
        .Tag("location", "Kitchen")
        .Field("temperature", temperature1)
        .Timestamp(now, WritePrecision.S);

    var temperature2 = random.Next(20, 30);
    var point2 = PointData
        .Measurement("sensor2")
        .Tag("location", "Bedroom")
        .Field("temperature", temperature2)
        .Timestamp(now, WritePrecision.S);

    var points = new List<PointData>() { point1, point2 };
    var response = await writeApi.WritePointsAsyncWithIRestResponse(points, bucket, org);
    Console.WriteLine($"{now}: {(int)response.First().StatusCode} - temperature1: {temperature1}, temperature2: {temperature2}");
    await Task.Delay(1000);
}


var mem = new Mem { Host = "host1", UsedPercent = 23.43234543, Time = DateTime.UtcNow };

//using (var writeApi = client.GetWriteApi())
//{
//    writeApi.WriteMeasurement(bucket, WritePrecision.Ns, org, mem);
//}

var query = "from(bucket: \"demo\") |> range(start: -1h)";
var tables = await client.GetQueryApi().QueryAsync(query, org);

foreach (var record in tables.SelectMany(table => table.Records))
{
    Console.WriteLine($"{record}");
}

[Measurement("mem")]
class Mem
{
    [Column("host", IsTag = true)] public string Host { get; set; }
    [Column("used_percent")] public double? UsedPercent { get; set; }
    [Column(IsTimestamp = true)] public DateTime Time { get; set; }
}