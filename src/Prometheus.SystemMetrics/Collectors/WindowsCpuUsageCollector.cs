using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
			{ "% Privileged Time", "system" },
			{ "% Interrupt Time", "irq" },
			{ "% Idle Time", "idle" },
			{ "% User Time", "user" }
		};

		private const int NS_IN_SEC = 1000000000;
		private const int COUNTER_100NS_IN_SEC = NS_IN_SEC / 100;

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
				// These are all Timer100Ns type
				_counters.Add(new PerformanceCounter("Processor", "% Privileged Time", instanceName)); //system
				_counters.Add(new PerformanceCounter("Processor", "% Interrupt Time", instanceName)); //iowait
				_counters.Add(new PerformanceCounter("Processor", "% Idle Time", instanceName)); //idle
				_counters.Add(new PerformanceCounter("Processor", "% User Time", instanceName)); //user
			}

			var hasUnexpectedType = _counters.Any(x => x.CounterType != PerformanceCounterType.Timer100Ns);
			if (hasUnexpectedType)
			{
				throw new Exception(
					"A CPU counter was not of type Timer100Ns. Please report a bug to the prometheus-net.SystemMetrics project"
				);
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
				// Assumes all counters are Timer100Ns type
				var value = (float)performanceCounter.RawValue / COUNTER_100NS_IN_SEC;

				Cpu.WithLabels(
					performanceCounter.InstanceName,
					_labels[performanceCounter.CounterName]
				).IncTo(value);
			}
		}
	}
}
