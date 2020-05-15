using System.Collections.Generic;
using System.IO;

namespace Prometheus.SystemMetrics.Collectors
{
	/// <summary>
	/// Configuration for <see cref="DiskCollector"/>
	/// </summary>
	public class DiskCollectorConfig
	{
		/// <summary>
		/// Drive types to collect data on. By default, only collects data for "fixed" drives.
		/// </summary>
		public HashSet<DriveType> DriveTypes { get; set; } = new HashSet<DriveType> { DriveType.Fixed };
	}
}
