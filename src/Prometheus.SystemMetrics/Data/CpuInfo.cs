using System;

namespace Prometheus.SystemMetrics.Data
{
	public class CpuInfo
	{
		public DateTime LastUpdate { get; set; }
		public TimeSpan LastProcTime { get; set; }
		public double LastPercent { get; set; }
		public long MemorySize { get; set; }
	}
}
