using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Prometheus.SystemMetrics.Parsers
{
	/// <summary>
	/// Parser for /proc/meminfo files
	/// </summary>
	public static class MemInfoParser
	{
		/// <summary>
		/// Location of the meminfo file
		/// </summary>
		public const string MEMINFO_FILE = "/proc/meminfo";

		private const int BYTES_IN_KB = 1024;

		/// <summary>
		/// eg. "MemTotal:      12345 kB"
		/// </summary>
		private static readonly Regex _lineRegex = new Regex(
			@"^(?<name>[^:]+):\s+(?<value>\d+) kB",
			RegexOptions.Multiline | RegexOptions.Compiled
		);

		/// <summary>
		/// Parses the /proc/meminfo file.
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<(string field, ulong value)> Parse()
		{
			return Parse(File.ReadAllText(MEMINFO_FILE));
		}

		/// <summary>
		/// Parses the specified contents of the meminfo file.
		/// </summary>
		/// <param name="file">Content to parse</param>
		/// <returns>Parsed rows</returns>
		public static IEnumerable<(string field, ulong value)> Parse(string file)
		{
			return _lineRegex.Matches(file)
				.Cast<Match>()
				.Select(
					match => (
						match.Groups["name"].Value.TrimStart(),
						ulong.Parse(match.Groups["value"].Value) * BYTES_IN_KB
					)
				);
		}
	}
}
