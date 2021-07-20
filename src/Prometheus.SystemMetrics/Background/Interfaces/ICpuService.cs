namespace Prometheus.SystemMetrics.Background.Interfaces
{
	public interface ICpuService
	{
		double GetCpu(int pid);
		long GetMemory(int pid);
	}
}
