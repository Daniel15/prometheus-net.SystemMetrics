using System.Linq;

namespace Prometheus.SystemMetrics.Collectors
{
	/// <summary>
	/// Wraps multiple metric collectors and uses the first one that is supported by the system.
	/// </summary>
	public abstract class MetricCollectorFacade : ISystemMetricCollector
	{
		private ISystemMetricCollector? _innerCollector = null;

		/// <summary>
		/// Creates a new <see cref="MetricCollectorFacade"/>
		/// </summary>
		public MetricCollectorFacade()
		{
			_innerCollector = Collectors.FirstOrDefault(x => x.IsSupported);
		}

		/// <summary>
		/// Gets whether this metric is supported on the current system.
		/// </summary>
		public bool IsSupported => _innerCollector != null;
		
		/// <summary>
		/// Creates the Prometheus metric.
		/// </summary>
		/// <param name="factory">Factory to create metric using</param>
		public void CreateMetrics(MetricFactory factory)
		{
			_innerCollector?.CreateMetrics(factory);
		}

		/// <summary>
		/// Update data for the metrics. Called immediately before the metrics are scraped.
		/// </summary>
		public void UpdateMetrics()
		{
			_innerCollector?.UpdateMetrics();
		}

		/// <summary>
		/// All the collectors to try. The first one that returns <c>true</c> for
		/// <see cref="IsSupported"/> will be used.
		/// </summary>
		protected abstract ISystemMetricCollector[] Collectors { get; }
	}
}
