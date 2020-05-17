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
		public static IEnumerable<CpuUsageData> ParseCpuUsage(string stats, int clockTicksPerSecond)
		{
			return stats
				.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
				.Where(x => x.StartsWith("cpu") && !x.StartsWith("cpu "))
				.Select(line => new CpuUsageData(line, clockTicksPerSecond));
		}

		/// <summary>
		/// Represents CPU usage data from /proc/stat
		/// </summary>
		/// <remarks>
		/// Docs: http://man7.org/linux/man-pages/man5/proc.5.html
		/// </remarks>
		public readonly struct CpuUsageData
		{
			/// <summary>
			/// Parses a line from /proc/stat
			/// </summary>
			public CpuUsageData(string rawLine, int clockTicksPerSecond)
			{
				double ParseCpuStat(string rawValue)
				{
					return double.Parse(rawValue) / clockTicksPerSecond;
				}

				var pieces = rawLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				CpuIndex = ushort.Parse(pieces[0].Replace("cpu", string.Empty));
				User = ParseCpuStat(pieces[1]);
				Nice = ParseCpuStat(pieces[2]);
				System = ParseCpuStat(pieces[3]);
				Idle = ParseCpuStat(pieces[4]);
				IoWait = ParseCpuStat(pieces[5]);
				Irq = ParseCpuStat(pieces[6]);
				SoftIrq = ParseCpuStat(pieces[7]);
				Steal = ParseCpuStat(pieces[8]);
			}

			/// <summary>
			/// The index of the CPU. Starts at 0 for the first CPU.
			/// </summary>
			public ushort CpuIndex { get; }

			/// <summary>
			/// Time spent in the idle task.  This value should be USER_HZ times the second entry in the /proc/uptime pseudo-file.
			/// </summary>
			public double Idle { get; }

			/// <summary>
			/// Time waiting for I/O to complete.
			/// </summary>
			public double IoWait { get; }

			/// <summary>
			/// Time servicing interrupts.
			/// </summary>
			public double Irq { get; }
			/// <summary>
			/// Time spent in user mode with low priority (nice).
			/// </summary>
			public double Nice { get; }

			/// <summary>
			/// Time servicing softirqs.
			/// </summary>
			public double SoftIrq { get; }

			/// <summary>
			/// Time spent in system mode.
			/// </summary>
			public double System { get; }

			/// <summary>
			/// Stolen time, which is the time spent in other operating systems when running in a virtualized environment
			/// </summary>
			public double Steal { get; }

			/// <summary>
			/// Time spent in user mode.
			/// </summary>
			public double User { get; }
		}
	}
}
