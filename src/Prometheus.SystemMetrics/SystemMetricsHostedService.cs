using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Prometheus.SystemMetrics.Collectors;

namespace Prometheus.SystemMetrics
{
	/// <summary>
	/// Handles registering SystemMetrics with Prometheus on app startup.
	/// </summary>
	public class SystemMetricsHostedService : IHostedService
	{
		private readonly IEnumerable<ISystemMetricCollector> _collectors;

		public SystemMetricsHostedService(IEnumerable<ISystemMetricCollector> collectors)
		{
			_collectors = collectors;
		}
		/// <summary>
		/// Triggered when the application host is ready to start the service.
		/// </summary>
		/// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
		public Task StartAsync(CancellationToken cancellationToken)
		{
			// TODO: Should allow injecting a custom registry
			var registry = Metrics.DefaultRegistry;
			var factory = Metrics.WithCustomRegistry(registry);
			foreach (var collector in _collectors.Where(collector => collector.IsSupported))
			{
				collector.CreateMetrics(factory);
				registry.AddBeforeCollectCallback(collector.UpdateMetrics);
			}

			return Task.CompletedTask;
		}

		/// <summary>
		/// Triggered when the application host is performing a graceful shutdown.
		/// </summary>
		/// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}
