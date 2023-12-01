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

## Customization

By default, this will add all the collectors. To only add some collectors, you can instead only render the collectors you want to use:

```csharp
services.AddSystemMetrics(registerDefaultCollectors: false);
services.AddSystemMetricCollector<CpuUsageCollector>();
```

# Metrics

Where possible, metrics have the same name and format as `node_exporter`.

## CPU

The number of seconds the CPU has spent in each mode (system, user, idle, etc). Available on **Linux** and **Windows**. Example data:

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

## Memory

Stats such as available RAM, RAM used by caches, etc. Available on **Linux** and **Windows**. Example data:

```
node_memory_MemAvailable_bytes 1527701504
node_memory_Cached_bytes 572964864
node_memory_MemFree_bytes 961966080
node_memory_MemTotal_bytes 2085904384
```

## Network

Total amount of data sent and received over the network. Available on **all platforms**. Example data:

```
node_network_transmit_bytes_total{device="eth0"} 3053723
node_network_receive_bytes_total{device="eth0"} 5822231
```

# Changelog

## 3.0.0 - 30th November 2023

* Bumped to .NET 8.0.
* Bumped prometheus-net dependency to version 8.
* Wrapped metric collector creation in try-catch so that one collector failing doesn't break the whole app.
* Updated Windows memory counters so their names more closely match the Linux version. Notably, `node_memory_MemFree` is now `node_memory_MemAvailable_bytes`, and `node_memory_MemTotal` is now `node_memory_MemTotal_bytes`. Currently, both the old and new counters exist (for backwards compatibility), but the old ones will be removed in the next major version.

## 2.0.0 - 8th October 2021

* Added memory and CPU collectors for Windows (thanks to @masterworgen for the initial implementation in PR #3).
* Added .NET Framework 4.6.2 builds, since prometheus-net itself supports this framework version.

## 1.0.1 - 17th May 2020

* Added memory stats for Linux.
* Added total file size to disk collector.
