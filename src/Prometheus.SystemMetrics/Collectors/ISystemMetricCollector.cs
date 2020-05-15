namespace Prometheus.SystemMetrics.Collectors
{
	/// <summary>
	/// Represents a mechanism for collecting some sort of metrics.
	/// </summary>
	public interface ISystemMetricCollector
	{
		/// <summary>
		/// Gets whether this metric is supported on the current system.
		/// </summary>
		bool IsSupported { get; }

		/// <summary>
		/// Creates the Prometheus metric.
		/// </summary>
		/// <param name="factory">Factory to create metric using</param>
		void CreateMetrics(MetricFactory factory);

		/// <summary>
		/// Update data for the metrics. Called immediately before the metrics are scraped.
		/// </summary>
		void UpdateMetrics();
	}
}
