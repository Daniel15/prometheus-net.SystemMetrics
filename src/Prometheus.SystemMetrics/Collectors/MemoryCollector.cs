using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Prometheus.SystemMetrics.Helper;
using Prometheus.SystemMetrics.Native;
using Prometheus.SystemMetrics.Parsers;

namespace Prometheus.SystemMetrics.Collectors
{
	/// <summary>
	/// Collects memory usage information.
	/// </summary>
	public class MemoryCollector : ISystemMetricCollector
	{
		/// <summary>
		/// Gets whether this metric is supported on the current system.
		/// </summary>
		public bool IsSupported => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || File.Exists(MemInfoParser.MEMINFO_FILE);

		/// <summary>
		/// Metrics for memory collection.
		/// </summary>
		internal IReadOnlyDictionary<string, Gauge> Metrics = new Dictionary<string, Gauge>();

		/// <summary>
		/// Creates the Prometheus metric.
		/// </summary>
		/// <param name="factory">Factory to create metric using</param>
		public void CreateMetrics(MetricFactory factory)
		{
			var metrics = new Dictionary<string, Gauge>();
			foreach (var label in MemInfoHelper.Labels)
			{
				var cleanName = $"node_memory_{label.Value}_bytes";
				metrics[label.Key] = factory.CreateGauge(cleanName, $"Memory information field {label.Value}");
			}
			Metrics = metrics;
		}

		/// <summary>
		/// Update data for the metrics. Called immediately before the metrics are scraped.
		/// </summary>
		public void UpdateMetrics()
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				UpdateMetricsLinux();
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				UpdateMetricsWindows();
		}

		private void UpdateMetricsLinux()
		{
			foreach (var (name, value) in MemInfoParser.Parse())
			{
				if (Metrics.TryGetValue(name.ToUpper(), out var gauge))
				{
					gauge.Set(value);
				}
			}
		}

		private void UpdateMetricsWindows()
		{
			var memStatus = new MEMORYSTATUSEX();
			
			if (!WindowsNative.GlobalMemoryStatusEx(memStatus))
				throw new Exception(Marshal.GetLastWin32Error().ToString());

			Metrics[nameof(memStatus.dwMemoryLoad).ToUpper()]?.Set(memStatus.dwMemoryLoad);
			Metrics[nameof(memStatus.ullAvailExtendedVirtual).ToUpper()]?.Set(memStatus.ullAvailExtendedVirtual);
			Metrics[nameof(memStatus.ullAvailPageFile).ToUpper()]?.Set(memStatus.ullAvailPageFile);
			Metrics[nameof(memStatus.ullAvailPhys).ToUpper()]?.Set(memStatus.ullAvailPhys);
			Metrics[nameof(memStatus.ullAvailVirtual).ToUpper()]?.Set(memStatus.ullAvailVirtual);
			Metrics[nameof(memStatus.ullTotalPageFile).ToUpper()]?.Set(memStatus.ullTotalPageFile);
			Metrics[nameof(memStatus.ullTotalPhys).ToUpper()]?.Set(memStatus.ullTotalPhys);
			Metrics[nameof(memStatus.ullTotalVirtual).ToUpper()]?.Set(memStatus.ullTotalVirtual);
		}
	}
}
