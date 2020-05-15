using System;
using System.Collections.Generic;
using System.Linq;

namespace Prometheus.SystemMetrics.Parsers
{
	/// <summary>
	/// Handles parsing Linux /proc/stat file.
	/// </summary>
	/// <remarks>
	/// Docs: http://man7.org/linux/man-pages/man5/proc.5.html
	/// </remarks>
	public static class ProcStatParser
	{
		/// <summary>
		/// Parses CPU usage data from /proc/stat
		/// </summary>
		/// <param name="stats">Raw data</param>
		/// <param name="clockTicksPerSecond">Number of clock ticks per second (from _SC_CLK_TCK)</param>
		/// <returns>Nicely-formatted CPU usage data</returns>
		public static IDictionary<string, CpuUsageData> ParseCpuUsage(string stats, int clockTicksPerSecond)
		{
			double ParseCpuStat(string rawValue)
			{
				return double.Parse(rawValue) / clockTicksPerSecond;
			}

			var lines = stats
				.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
				.Where(x => x.StartsWith("cpu") && !x.StartsWith("cpu "));

			var result = new Dictionary<string, CpuUsageData>();
			foreach (var line in lines)
			{
				var pieces = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				var cpuIndex = pieces[0].Replace("cpu", string.Empty);
				result[cpuIndex] = new CpuUsageData
				{
					User = ParseCpuStat(pieces[1]),
					Nice = ParseCpuStat(pieces[2]),
					System = ParseCpuStat(pieces[3]),
					Idle = ParseCpuStat(pieces[4]),
					IoWait = ParseCpuStat(pieces[5]),
					Irq = ParseCpuStat(pieces[6]),
					SoftIrq = ParseCpuStat(pieces[7]),
					Steal = ParseCpuStat(pieces[8]),
				};
			}

			return result;
		}

		/// <summary>
		/// Represents CPU usage data from /proc/stat
		/// </summary>
		public class CpuUsageData
		{
			public double Idle { get; set; }
			public double IoWait { get; set; }
			public double Irq { get; set; }
			public double Nice { get; set; }
			public double SoftIrq { get; set; }
			public double System { get; set; }
			public double Steal { get; set; }
			public double User { get; set; }
		}
	}
}
