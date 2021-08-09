using System;

namespace Prometheus.SystemMetrics.Collectors
{
	/// <summary>
	/// Collects data on overall CPU usage
	/// </summary>
	[Obsolete("Use LinuxCpuUsageCollector or WindowsCpuUsageCollector")]
	public class CpuUsageCollector : MetricCollectorFacade
	{
		/// <summary>
		/// All the collectors to try. The first one that returns <c>true</c> for
		/// IsSupported will be used.
		/// </summary>
		protected override ISystemMetricCollector[] Collectors => new ISystemMetricCollector[]
		{
			new LinuxCpuUsageCollector(),
			new WindowsCpuUsageCollector(),
		};
	}
}
