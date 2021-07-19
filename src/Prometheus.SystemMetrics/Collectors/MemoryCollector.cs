using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
		public bool IsSupported => true;

		/// <summary>
		/// Metrics for memory collection.
		/// </summary>
		internal IReadOnlyDictionary<string, Gauge> _metrics = new Dictionary<string, Gauge>();

		/// <summary>
		/// Creates the Prometheus metric.
		/// </summary>
		/// <param name="factory">Factory to create metric using</param>
		public void CreateMetrics(MetricFactory factory)
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				CreateMetricsLinux(factory);
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				CreateMetricsWindows(factory);
		}

		private void CreateMetricsLinux(MetricFactory factory)
		{
			var metrics = new Dictionary<string, Gauge>();
			foreach (var (name, _) in MemInfoParser.Parse())
			{
				var cleanName = name.Replace('(', '_').Replace(")", "");
				cleanName = $"node_memory_{cleanName}_bytes";
				metrics[name] = factory.CreateGauge(cleanName, $"Memory information field {name}");
			}
			_metrics = metrics;
		}

		private void CreateMetricsWindows(MetricFactory factory)
		{
			var metrics = new Dictionary<string, Gauge>();

			metrics[nameof(MEMORYSTATUSEX.dwLength)] = factory.CreateGauge($"node_memory_{nameof(MEMORYSTATUSEX.dwLength)}_bytes", $"Memory information field {nameof(MEMORYSTATUSEX.dwLength)}");
			metrics[nameof(MEMORYSTATUSEX.dwMemoryLoad)] = factory.CreateGauge($"node_memory_{nameof(MEMORYSTATUSEX.dwMemoryLoad)}_bytes", $"Memory information field {nameof(MEMORYSTATUSEX.dwMemoryLoad)}");
			metrics[nameof(MEMORYSTATUSEX.ullAvailExtendedVirtual)] = factory.CreateGauge($"node_memory_{nameof(MEMORYSTATUSEX.ullAvailExtendedVirtual)}_bytes", $"Memory information field {nameof(MEMORYSTATUSEX.ullAvailExtendedVirtual)}");
			metrics[nameof(MEMORYSTATUSEX.ullAvailPageFile)] = factory.CreateGauge($"node_memory_{nameof(MEMORYSTATUSEX.ullAvailPageFile)}_bytes", $"Memory information field {nameof(MEMORYSTATUSEX.ullAvailPageFile)}");
			metrics[nameof(MEMORYSTATUSEX.ullAvailPhys)] = factory.CreateGauge($"node_memory_{nameof(MEMORYSTATUSEX.ullAvailPhys)}_bytes", $"Memory information field {nameof(MEMORYSTATUSEX.ullAvailPhys)}");
			metrics[nameof(MEMORYSTATUSEX.ullAvailVirtual)] = factory.CreateGauge($"node_memory_{nameof(MEMORYSTATUSEX.ullAvailVirtual)}_bytes", $"Memory information field {nameof(MEMORYSTATUSEX.ullAvailVirtual)}");
			metrics[nameof(MEMORYSTATUSEX.ullTotalPageFile)] = factory.CreateGauge($"node_memory_{nameof(MEMORYSTATUSEX.ullTotalPageFile)}_bytes", $"Memory information field {nameof(MEMORYSTATUSEX.ullTotalPageFile)}");
			metrics[nameof(MEMORYSTATUSEX.ullTotalPhys)] = factory.CreateGauge($"node_memory_{nameof(MEMORYSTATUSEX.ullTotalPhys)}_bytes", $"Memory information field {nameof(MEMORYSTATUSEX.ullTotalPhys)}");
			metrics[nameof(MEMORYSTATUSEX.ullTotalVirtual)] = factory.CreateGauge($"node_memory_{nameof(MEMORYSTATUSEX.ullTotalVirtual)}_bytes", $"Memory information field {nameof(MEMORYSTATUSEX.ullTotalVirtual)}");

			_metrics = metrics;
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
				if (_metrics.TryGetValue(name, out var gauge))
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

			_metrics[nameof(memStatus.dwLength)]?.Set(memStatus.dwLength);
			_metrics[nameof(memStatus.dwMemoryLoad)]?.Set(memStatus.dwMemoryLoad);
			_metrics[nameof(memStatus.ullAvailExtendedVirtual)]?.Set(memStatus.ullAvailExtendedVirtual);
			_metrics[nameof(memStatus.ullAvailPageFile)]?.Set(memStatus.ullAvailPageFile);
			_metrics[nameof(memStatus.ullAvailPhys)]?.Set(memStatus.ullAvailPhys);
			_metrics[nameof(memStatus.ullAvailVirtual)]?.Set(memStatus.ullAvailVirtual);
			_metrics[nameof(memStatus.ullTotalPageFile)]?.Set(memStatus.ullTotalPageFile);
			_metrics[nameof(memStatus.ullTotalPhys)]?.Set(memStatus.ullTotalPhys);
			_metrics[nameof(memStatus.ullTotalVirtual)]?.Set(memStatus.ullTotalVirtual);
		}
	}
}
