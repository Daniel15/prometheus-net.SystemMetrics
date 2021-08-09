using System.Collections.Generic;
using System.IO;
using Prometheus.SystemMetrics.Parsers;

namespace Prometheus.SystemMetrics.Collectors
{
	/// <summary>
	/// Collects memory usage information, on Linux.
	/// </summary>
	public class LinuxMemoryCollector : ISystemMetricCollector
	{
		/// <summary>
		/// Gets whether this metric is supported on the current system.
		/// </summary>
		public bool IsSupported => File.Exists(MemInfoParser.MEMINFO_FILE);

		/// <summary>
		/// Metrics for memory collection.
		/// </summary>
		internal IReadOnlyDictionary<string, Gauge> _metrics = new Dictionary<string, Gauge>();

		/// <summary>
		/// Creates the Prometheus metric.
		/// </summary>
		/// <param name="factory">Factory to create metric using</param>
		public void CreateMetrics(MetricFactory factory)
		{
			var metrics = new Dictionary<string, Gauge>();
			foreach (var (name, _) in MemInfoParser.Parse())
			{
				var cleanName = name.Replace('(', '_').Replace(")", "");
				cleanName = $"node_memory_{cleanName}_bytes";
				metrics[name] = factory.CreateGauge(cleanName, $"Memory information field {name}");
			}
			_metrics = metrics;
		}

		/// <summary>
		/// Update data for the metrics. Called immediately before the metrics are scraped.
		/// </summary>
		public void UpdateMetrics()
		{
			foreach (var (name, value) in MemInfoParser.Parse())
			{
				if (_metrics.TryGetValue(name, out var gauge))
				{
					gauge.Set(value);
				}
			}
		}
	}
}
