using Microsoft.Extensions.DependencyInjection;
using Prometheus.SystemMetrics.Collectors;

namespace Prometheus.SystemMetrics
{
	/// <summary>
	/// Adds SystemMetrics classes to the <see cref="IServiceCollection"/>.
	/// </summary>
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		/// Adds the SystemMetrics classes to the <see cref="IServiceCollection"/>.
		/// </summary>
		public static IServiceCollection AddSystemMetrics(this IServiceCollection services, bool registerDefaultCollectors = true)
		{
			services.AddHostedService<SystemMetricsHostedService>();
			services.AddOptions<DiskCollectorConfig>();

			if (registerDefaultCollectors)
			{
				services.AddSystemMetricCollector<CpuUsageCollector>();
				services.AddSystemMetricCollector<DiskCollector>();
				services.AddSystemMetricCollector<LoadAverageCollector>();
				services.AddSystemMetricCollector<MemoryCollector>();
				services.AddSystemMetricCollector<NetworkCollector>();
			}

			return services;
		}

		/// <summary>
		/// Adds a system metric collector to the <see cref="IServiceCollection"/>
		/// </summary>
		/// <typeparam name="T">Metric to add</typeparam>
		public static IServiceCollection AddSystemMetricCollector<T>(this IServiceCollection services)
			where T : class, ISystemMetricCollector
		{
			services.AddSingleton<ISystemMetricCollector, T>();
			return services;
		}
	}
}
