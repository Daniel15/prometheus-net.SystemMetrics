using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Mono.Unix.Native;
using Prometheus.SystemMetrics.Background;
using Prometheus.SystemMetrics.Background.Interfaces;
using Prometheus.SystemMetrics.Parsers;

namespace Prometheus.SystemMetrics.Collectors
{
	/// <summary>
	/// Collects data on overall CPU usage
	/// </summary>
	public class CpuUsageCollector : ISystemMetricCollector
	{
		private readonly ICpuService _cpuService;

		public CpuUsageCollector(ICpuService cpuService)
		{
			_cpuService = cpuService;
		}

		/// <summary>
		/// File to read stats from
		/// </summary>
		private const string STAT_FILE = "/proc/stat";

		internal Counter CpuLinux { get; private set; } = default!;
		internal Gauge CpuWindows { get; private set; } = default!;

		private int _clockTicksPerSecond = 100;

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
			CpuLinux = factory.CreateCounter(
				"node_cpu_seconds_total",
				"Seconds the CPU spent in each mode",
				"cpu",
				"mode"
			);

			_clockTicksPerSecond = (int)Syscall.sysconf(SysconfName._SC_CLK_TCK);
		}

		private void CreateMetricsWindows(MetricFactory factory)
		{
			CpuWindows = factory.CreateGauge(
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
				CpuLinux.WithLabels(cpuIndex, "idle").IncTo(data.Idle);
				CpuLinux.WithLabels(cpuIndex, "iowait").IncTo(data.IoWait);
				CpuLinux.WithLabels(cpuIndex, "irq").IncTo(data.Irq);
				CpuLinux.WithLabels(cpuIndex, "nice").IncTo(data.Nice);
				CpuLinux.WithLabels(cpuIndex, "softirq").IncTo(data.SoftIrq);
				CpuLinux.WithLabels(cpuIndex, "steal").IncTo(data.Steal);
				CpuLinux.WithLabels(cpuIndex, "system").IncTo(data.System);
				CpuLinux.WithLabels(cpuIndex, "user").IncTo(data.User);
			}
		}

		private void UpdateMetricsWindows()
		{
			CpuWindows.WithLabels("0", "total used").Set(CpuService.CpuUsed);
			CpuWindows.WithLabels("0", "component used").Set(_cpuService.GetCpu(Process.GetCurrentProcess().Id));
		}
	}
}
