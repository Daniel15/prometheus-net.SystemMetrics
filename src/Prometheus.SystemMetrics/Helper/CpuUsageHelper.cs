using System.Collections.Generic;

namespace Prometheus.SystemMetrics.Helper
{
	public class CpuUsageHelper
	{
		public static readonly Dictionary<string, string> Labels = new Dictionary<string, string>
		{
			{ "% Processor Time", "system" },
			{ "Interrupts/sec", "irq" },
			{ "% Interrupt Time", "iowait" },
			{ "% Idle Time", "idle" },
			{ "% User Time", "user" }
		};
	}
}
