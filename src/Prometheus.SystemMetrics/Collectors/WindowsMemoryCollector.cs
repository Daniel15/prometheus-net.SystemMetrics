using System;
using System.Runtime.InteropServices;
using Prometheus.SystemMetrics.Native;

namespace Prometheus.SystemMetrics.Collectors
{
	/// <summary>
	/// Collects memory usage information, on Windows.
	/// </summary>
	public class WindowsMemoryCollector : ISystemMetricCollector
	{
		/// <summary>
		/// Gets whether this metric is supported on the current system.
		/// </summary>
#if NETFRAMEWORK
		public bool IsSupported => true;
#else
		public bool IsSupported => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#endif

		internal Gauge MemoryLoad { get; private set; } = default!;
		internal Gauge PageFileAvailable { get; private set; } = default!;
		internal Gauge PageFileTotal { get; private set; } = default!;
		internal Gauge PhysicalAvailable { get; private set; } = default!;
		internal Gauge PhysicalTotal { get; private set; } = default!;
		internal Gauge VirtualAvailable { get; private set; } = default!;
		internal Gauge VirtualTotal { get; private set; } = default!;

		internal Gauge LegacyPageFileAvailable { get; private set; } = default!;
		internal Gauge LegacyPageFileTotal { get; private set; } = default!;
		internal Gauge LegacyPhysicalAvailable { get; private set; } = default!;
		internal Gauge LegacyPhysicalTotal { get; private set; } = default!;
		internal Gauge LegacyVirtualAvailable { get; private set; } = default!;
		internal Gauge LegacyVirtualTotal { get; private set; } = default!;


		/// <summary>
		/// Creates the Prometheus metric.
		/// </summary>
		/// <param name="factory">Factory to create metric using</param>
		public void CreateMetrics(MetricFactory factory)
		{
			MemoryLoad = Metrics.CreateGauge(
				"node_memory_MemoryLoad",
				"A number between 0 and 100 that specifies the approximate percentage of physical memory that is in use (0 indicates no memory use and 100 indicates full memory use)."
			);

			PageFileAvailable = Metrics.CreateGauge(
				"node_memory_PageFileFree_bytes",
				"The maximum amount of memory the current process can commit, in bytes."
			);
			PageFileTotal = Metrics.CreateGauge(
				"node_memory_PageFileTotal_bytes",
				"The current committed memory limit for the system or the current process, whichever is smaller, in bytes."
			);

			PhysicalAvailable = Metrics.CreateGauge(
				"node_memory_MemAvailable_bytes",
				"The amount of physical memory currently available, in bytes. This is the amount of physical memory that can be immediately reused without having to write its contents to disk first."
			);
			PhysicalTotal = Metrics.CreateGauge(
				"node_memory_MemTotal_bytes",
				"The amount of actual physical memory, in bytes."
			);

			VirtualAvailable = Metrics.CreateGauge(
				"node_memory_VirtualFree_bytes",
				"The amount of unreserved and uncommitted memory currently in the user-mode portion of the virtual address space of the calling process, in bytes."
			);
			VirtualTotal = Metrics.CreateGauge(
				"node_memory_VirtualTotal_bytes",
				" The size of the user-mode portion of the virtual address space of the calling process, in bytes. This value depends on the type of process, the type of processor, and the configuration of the operating system. For example, this value is approximately 2 GB for most 32-bit processes on an x86 processor and approximately 3 GB for 32-bit processes that are large address aware running on a system with 4-gigabyte tuning"
			);

			LegacyPageFileAvailable = Metrics.CreateGauge(
				"node_memory_PageFileFree",
				"DEPRECATED: Use node_memory_PageFileFree_bytes instead."
			);
			LegacyPageFileTotal = Metrics.CreateGauge(
				"node_memory_PageFileTotal",
				"DEPRECATED: Use node_memory_PageFileTotal_bytes instead."
			);

			LegacyPhysicalAvailable = Metrics.CreateGauge(
				"node_memory_MemFree",
				"DEPRECATED: Use node_memory_MemAvailable_bytes instead."
			);
			LegacyPhysicalTotal = Metrics.CreateGauge(
				"node_memory_MemTotal",
				"DEPRECATED: Use node_memory_MemTotal_bytes instead."
			);

			LegacyVirtualAvailable = Metrics.CreateGauge(
				"node_memory_VirtualFree",
				"DEPRECATED: Use node_memory_VirtualFree_bytes instead."
			);
			LegacyVirtualTotal = Metrics.CreateGauge(
				"node_memory_VirtualTotal",
				"DEPRECATED: Use node_memory_VirtualTotal_bytes instead"
			);
		}

		/// <summary>
		/// Update data for the metrics. Called immediately before the metrics are scraped.
		/// </summary>
		public void UpdateMetrics()
		{
			var memStatus = new MemoryStatusEx();

			if (!WindowsNative.GlobalMemoryStatusEx(ref memStatus))
			{
				throw new Exception(Marshal.GetLastWin32Error().ToString());
			}

			MemoryLoad.Set(memStatus.dwMemoryLoad);
			PageFileAvailable.Set(memStatus.ullAvailPageFile);
			PageFileTotal.Set(memStatus.ullTotalPageFile);
			PhysicalAvailable.Set(memStatus.ullAvailPhys);
			PhysicalTotal.Set(memStatus.ullTotalPhys);
			VirtualAvailable.Set(memStatus.ullAvailVirtual);
			VirtualTotal.Set(memStatus.ullTotalVirtual);

			LegacyPageFileAvailable.Set(memStatus.ullAvailPageFile);
			LegacyPageFileTotal.Set(memStatus.ullTotalPageFile);
			LegacyPhysicalAvailable.Set(memStatus.ullAvailPhys);
			LegacyPhysicalTotal.Set(memStatus.ullTotalPhys);
			LegacyVirtualAvailable.Set(memStatus.ullAvailVirtual);
			LegacyVirtualTotal.Set(memStatus.ullTotalVirtual);
		}
	}
}
