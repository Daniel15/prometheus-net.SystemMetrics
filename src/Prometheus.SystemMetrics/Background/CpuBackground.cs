using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Prometheus.SystemMetrics.Background.Interfaces;

namespace Prometheus.SystemMetrics.Background
{
	public class CpuBackground : BackgroundService
	{
		private readonly ICpuManager _cpuManager;

		public CpuBackground(ICpuManager cpuManager)
		{
			_cpuManager = cpuManager;
		}

		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			return _cpuManager.RunUpdateAsync(stoppingToken);
		}
	}
}
