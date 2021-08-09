using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Prometheus.SystemMetrics.Collectors
{
	/// <summary>
	/// Collects data on overall CPU usage, on Windows
	/// </summary>
	public class WindowsCpuUsageCollector : ISystemMetricCollector
	{
		internal Counter Cpu { get; private set; } = default!;

		private IList<PerformanceCounter> _counters = new List<PerformanceCounter>();

		/// <summary>
		/// Gets whether this metric is supported on the current system.
		/// </summary>
#if NETFRAMEWORK
		public bool IsSupported => true;
#else
		public bool IsSupported => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#endif

		private static readonly Dictionary<string, string> _labels = new()
		{
			{ "% Processor Time", "system" },
			{ "Interrupts/sec", "irq" },
			{ "% Interrupt Time", "iowait" },
			{ "% Idle Time", "idle" },
			{ "% User Time", "user" }
		};

		/// <summary>
		/// Creates the Prometheus metric.
		/// </summary>
		/// <param name="factory">Factory to create metric using</param>
		public void CreateMetrics(MetricFactory factory)
		{
			_counters = new List<PerformanceCounter>();

			for (var i = 0; i < Environment.ProcessorCount; i++)
			{
				var instanceName = $"{i}";
				_counters.Add(new PerformanceCounter("Processor", "% Processor Time", instanceName)); //system
				_counters.Add(new PerformanceCounter("Processor", "Interrupts/sec", instanceName)); //irq
				_counters.Add(new PerformanceCounter("Processor", "% Interrupt Time", instanceName)); //iowait
				_counters.Add(new PerformanceCounter("Processor", "% Idle Time", instanceName)); //idle
				_counters.Add(new PerformanceCounter("Processor", "% User Time", instanceName)); //user
			}

			Cpu = factory.CreateCounter(
				"node_cpu_seconds_total",
				"Seconds the CPU spent in each mode",
				"cpu",
				"mode"
			);
		}

		/// <summary>
		/// Update data for the metrics. Called immediately before the metrics are scraped.
		/// </summary>
		public void UpdateMetrics()
		{
			foreach (var performanceCounter in _counters)
			{
				Cpu.WithLabels(
					performanceCounter.InstanceName,
					_labels[performanceCounter.CounterName]
				).IncTo(performanceCounter.NextValue());
			}
		}
	}
}
