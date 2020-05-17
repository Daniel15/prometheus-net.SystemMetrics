using System.IO;
using Mono.Unix.Native;
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

		/// <summary>
		/// Gets whether this metric is supported on the current system.
		/// </summary>
		public bool IsSupported => File.Exists(STAT_FILE);

		/// <summary>
		/// Creates the Prometheus metric.
		/// </summary>
		/// <param name="factory">Factory to create metric using</param>
		public void CreateMetrics(MetricFactory factory)
		{
			Cpu = factory.CreateCounter(
				"node_cpu_seconds_total", 
				"Seconds the CPU spent in each mode", 
				"cpu", 
				"mode"
			);

			_clockTicksPerSecond = (int)Syscall.sysconf(SysconfName._SC_CLK_TCK);
		}

		/// <summary>
		/// Update data for the metrics. Called immediately before the metrics are scraped.
		/// </summary>
		public void UpdateMetrics()
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
	}
}
