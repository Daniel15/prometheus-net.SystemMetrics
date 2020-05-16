# prometheus-net SystemMetrics

[![NuGet version](http://img.shields.io/nuget/v/prometheus-net.SystemMetrics.svg)](https://www.nuget.org/packages/prometheus-net.SystemMetrics/)&nbsp;
![Build Status](https://img.shields.io/github/workflow/status/Daniel15/prometheus-net.SystemMetrics/Build)

prometheus-net SystemMetrics allows you to export various system metrics (such as CPU usage, disk usage, etc) from your .NET application to Prometheus. It is designed to be a very lightweight alternative to `node_exporter`, only containing essential metrics. This is useful on systems with limited RAM or where it is easier to add this library to your app instead of deploying a totally separate service.

# Usage

Install the `prometheus-net.SystemMetrics` library using NuGet.

Add the services to your `Startup.cs`:

```csharp
using Prometheus.SystemMetrics;

public void ConfigureServices(IServiceCollection services)
{
    // ...
    services.AddSystemMetrics();
}
```

If you have not already done so, you will also need to expose the Prometheus metrics endpoint by calling `MapMetrics` in `Configure`:

```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    // ...
    app.UseRouting();
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapMetrics();
        // ...
    });
}
```

After doing this, going to `/metrics` should return the new metrics.

# Metrics

Where possible, metrics have the same name and format as `node_exporter`.

## CPU

The number of seconds the CPU has spent in each mode (system, user, idle, etc). Available on **Linux**. Example data:

```
node_cpu_seconds_total{cpu="0",mode="system"} 172.35
node_cpu_seconds_total{cpu="0",mode="user"} 292.27
node_cpu_seconds_total{cpu="0",mode="idle"} 30760.4
```

## Disk

The amount of free disk space on all mounts. Available on **all platforms**. Example data:

```
# Linux
node_filesystem_avail_bytes{mountpoint="/",fstype="ext4"} 57061916672

# Windows
node_filesystem_avail_bytes{mountpoint="C:\\",fstype="NTFS"} 101531594752
```

## Load Average

Available on **Linux**. Example data:

```
node_load1 0.06
node_load5 0.03
node_load15 0.26
```

## Network

Total amount of data sent and received over the network. Available on **all platforms**. Example data:

```
node_network_transmit_bytes_total{device="eth0"} 3053723
node_network_receive_bytes_total{device="eth0"} 5822231
```
