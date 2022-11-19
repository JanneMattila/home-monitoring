# Home Monitoring

Monitoring sensors using Zigbee

[Configure a Grafana Docker image](https://grafana.com/docs/grafana/latest/setup-grafana/configure-docker/)

```bash
# install grafana
mkdir -p /temp/grafana
docker run -d --name=grafana -p 3000:3000 -v "c:\temp\docker\grafana:/var/lib/grafana" -e "GF_INSTALL_PLUGINS=grafana-clock-panel,grafana-simple-json-datasource" grafana/grafana-oss

# Login to grafana: admin / admin


# https://hub.docker.com/_/influxdb
# https://docs.influxdata.com/influxdb/v2.5/install/?t=Docker
docker run -d --name influxdb -p 8086:8086 -v "c:\temp\docker\influxdb:/var/lib/influxdb2" influxdb:2.5.1-alpine

# Login to influxdb: admin / admin123




```