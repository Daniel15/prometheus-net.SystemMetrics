using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Prometheus.SystemMetrics.Collectors;

namespace Prometheus.SystemMetrics
{
	/// <summary>
	/// Handles registering SystemMetrics with Prometheus on app startup.
	/// </summary>
	public class SystemMetricsHostedService : IHostedService
	{
		private readonly IEnumerable<ISystemMetricCollector> _collectors;
		private readonly ILogger<SystemMetricsHostedService> _logger;

		/// <summary>
		/// Creates a new <see cref="SystemMetricsHostedService"/>
		/// </summary>
		public SystemMetricsHostedService(
			IEnumerable<ISystemMetricCollector> collectors,
			ILogger<SystemMetricsHostedService> logger
		)
		{
			_collectors = collectors;
			_logger = logger;
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
				try
				{
					collector.CreateMetrics(factory);
					registry.AddBeforeCollectCallback(collector.UpdateMetrics);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, $"Could not initialize {collector.GetType().Name}. Some metrics will be missing.");
				}
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
