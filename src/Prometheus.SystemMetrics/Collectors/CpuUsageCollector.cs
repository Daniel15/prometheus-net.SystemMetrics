using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Mono.Unix.Native;
using Prometheus.SystemMetrics.Helper;
using Prometheus.SystemMetrics.Parsers;

namespace Prometheus.SystemMetrics.Collectors
{
	/// <summary>
	/// Collects data on overall CPU usage
	/// </summary>
	public class CpuUsageCollector : ISystemMetricCollector
	{
		/// <summary>
		/// File to read stats from
		/// </summary>
		private const string STAT_FILE = "/proc/stat";

		internal Counter Cpu { get; private set; } = default!;

		private int _clockTicksPerSecond = 100;

		private object _counters;

		/// <summary>
		/// Gets whether this metric is supported on the current system.
		/// </summary>
		public bool IsSupported => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || File.Exists(STAT_FILE);

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
			Cpu = factory.CreateCounter(
				"node_cpu_seconds_total",
				"Seconds the CPU spent in each mode",
				"cpu",
				"mode"
			);

			_clockTicksPerSecond = (int)Syscall.sysconf(SysconfName._SC_CLK_TCK);
		}

		private void CreateMetricsWindows(MetricFactory factory)
		{
			var counters = new List<PerformanceCounter>();
			_counters = counters;

			for (var i = 0; i < Environment.ProcessorCount; i++)
			{
				counters.Add(new PerformanceCounter("Processor", "% Processor Time", $"{i}")); //system
				counters.Add(new PerformanceCounter("Processor", "Interrupts/sec", $"{i}")); //irq
				counters.Add(new PerformanceCounter("Processor", "% Interrupt Time", $"{i}")); //iowait
				counters.Add(new PerformanceCounter("Processor", "% Idle Time", $"{i}")); //idle
				counters.Add(new PerformanceCounter("Processor", "% User Time", $"{i}")); //user
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
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				UpdateMetricsLinux();
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				UpdateMetricsWindows();
		}

		private void UpdateMetricsLinux()
		{
			var usage = ProcStatParser.ParseCpuUsage(File.ReadAllText(STAT_FILE), _clockTicksPerSecond);
			foreach (var data in usage)
			{
				var cpuIndex = data.CpuIndex.ToString();
				Cpu.WithLabels(cpuIndex, "idle").IncTo(data.Idle);
				Cpu.WithLabels(cpuIndex, "iowait").IncTo(data.IoWait);
				Cpu.WithLabels(cpuIndex, "irq").IncTo(data.Irq);
				Cpu.WithLabels(cpuIndex, "nice").IncTo(data.Nice);
				Cpu.WithLabels(cpuIndex, "softirq").IncTo(data.SoftIrq);
				Cpu.WithLabels(cpuIndex, "steal").IncTo(data.Steal);
				Cpu.WithLabels(cpuIndex, "system").IncTo(data.System);
				Cpu.WithLabels(cpuIndex, "user").IncTo(data.User);
			}
		}

		private void UpdateMetricsWindows()
		{
			var counters = (List<PerformanceCounter>) _counters;

			foreach (var performanceCounter in counters)
				Cpu.WithLabels(performanceCounter.InstanceName, CpuUsageHelper.Labels[performanceCounter.CounterName]).IncTo(performanceCounter.NextValue());
		}
	}
}
