using System.Threading;
using System.Threading.Tasks;

namespace Prometheus.SystemMetrics.Background.Interfaces
{
	public interface ICpuManager
	{
		Task RunUpdateAsync(CancellationToken cancellationToken);
	}
}
