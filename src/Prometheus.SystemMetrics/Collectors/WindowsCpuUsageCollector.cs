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

		// {counter name => {cpu index => counter}
		private IDictionary<string, PerformanceCounter[]> _counters = 
			new Dictionary<string, PerformanceCounter[]>();

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
		/// Once a counter reaches this threshold, it's recreated.
		/// https://github.com/Daniel15/prometheus-net.SystemMetrics/issues/9
		/// </summary>
		private const float RECREATE_COUNTER_THRESHOLD = 4000000;

		/// <summary>
		/// Creates the Prometheus metric.
		/// </summary>
		/// <param name="factory">Factory to create metric using</param>
		public void CreateMetrics(MetricFactory factory)
		{
			if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				return;
			}

			// Create one counter per metric per CPU
			foreach (var kvp in _labels)
			{
				_counters[kvp.Key] = new PerformanceCounter[Environment.ProcessorCount];
				for (var i = 0; i < Environment.ProcessorCount; i++)
				{
					CreateCounter(kvp.Key, i);
				}
			}

			Cpu = factory.CreateCounter(
				"node_cpu_seconds_total",
				"Seconds the CPU spent in each mode",
				"cpu",
				"mode"
			);
		}

		private void CreateCounter(string counterName, int cpuIndex)
		{
			var oldCounter = _counters[counterName][cpuIndex];
			oldCounter?.Dispose();

			var newCounter = new PerformanceCounter("Processor", counterName, cpuIndex.ToString());
			if (newCounter.CounterType != PerformanceCounterType.Timer100Ns)
			{
				throw new Exception(
					"A CPU counter was not of type Timer100Ns. Please report a bug to the prometheus-net.SystemMetrics project"
				);
			}
			_counters[counterName][cpuIndex] = newCounter;
		}

		/// <summary>
		/// Update data for the metrics. Called immediately before the metrics are scraped.
		/// </summary>
		public void UpdateMetrics()
		{
			if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				return;
			}
			
			foreach (var kvp in _counters)
			{
				var cpuIndex = 0;
				foreach (var performanceCounter in kvp.Value)
				{
					// Assumes all counters are Timer100Ns type
					var value = (double)performanceCounter.RawValue / COUNTER_100NS_IN_SEC;
					Cpu.WithLabels(
						performanceCounter.InstanceName,
						_labels[performanceCounter.CounterName]
					).IncTo(value);

					// If this counter is getting large, recreate it. Prometheus correctly handles
					// counters rolling over, so this shouldn't cause any issues.
					// https://github.com/Daniel15/prometheus-net.SystemMetrics/issues/9
					if (value >= RECREATE_COUNTER_THRESHOLD)
					{
						CreateCounter(kvp.Key, cpuIndex);
					}
					cpuIndex++;
				}
			}
		}
	}
}
