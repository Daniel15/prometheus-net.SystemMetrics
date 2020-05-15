using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Prometheus.SystemMetrics.Collectors
{
	/// <summary>
	/// Collects data on disk usage.
	/// </summary>
	public class DiskCollector : ISystemMetricCollector
	{
		private readonly ILogger<DiskCollector> _logger;
		private readonly HashSet<DriveType> _driveTypes;

		internal Gauge DiskSpace { get; private set; } = default!;

		public DiskCollector(IOptions<DiskCollectorConfig> config, ILogger<DiskCollector> logger)
		{
			_logger = logger;
			_driveTypes = config.Value.DriveTypes;
		}

		/// <summary>
		/// Gets whether this metric is supported on the current system.
		/// </summary>
		public bool IsSupported => true;

		/// <summary>
		/// Creates the Prometheus metric.
		/// </summary>
		/// <param name="factory">Factory to create metric using</param>
		public void CreateMetrics(MetricFactory factory)
		{
			DiskSpace = factory.CreateGauge(
				"node_filesystem_avail_bytes",
				"Free disk space", 
				"mountpoint",
				"fstype"
			);
		}

		/// <summary>
		/// Update data for the metrics. Called immediately before the metrics are scraped.
		/// </summary>
		public void UpdateMetrics()
		{
			foreach (var drive in DriveInfo.GetDrives().Where(x => _driveTypes.Contains(x.DriveType)))
			{
				try
				{
					DiskSpace.WithLabels(drive.Name, drive.DriveFormat).Set(drive.AvailableFreeSpace);
				}
				catch (Exception ex)
				{
					_logger.LogWarning("Could not get information for disk {0}: {1}", drive.Name, ex.Message);
				}
			}
		}
	}
}
