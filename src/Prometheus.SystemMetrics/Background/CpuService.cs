using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Prometheus.SystemMetrics.Background.Interfaces;
using Prometheus.SystemMetrics.Data;

namespace Prometheus.SystemMetrics.Background
{
	public class CpuService : ICpuService, ICpuManager
	{
		private static CpuInfo CurrentProcess { get; } = new CpuInfo();

		public async Task RunUpdateAsync(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				var processes = Process.GetCurrentProcess();
				Calc(processes);

				await Task.Delay(1000, cancellationToken);
			}
		}

		public void Calc(Process proc)
		{
			try
			{
				if (proc.Id == 0)
					return;

				if (proc.HasExited)
					return;

				var curDateTime = DateTime.UtcNow;
				var interval = curDateTime - CurrentProcess.LastUpdate;
				var totalProcTime = proc.TotalProcessorTime;
				var curTime = totalProcTime - CurrentProcess.LastProcTime;
				var cpu = ((curTime.TotalMilliseconds / interval.TotalMilliseconds) / Environment.ProcessorCount) * 100;

				CurrentProcess.LastProcTime = totalProcTime;
				CurrentProcess.LastUpdate = curDateTime;
				CurrentProcess.LastPercent = cpu;
				CurrentProcess.MemorySize = proc.WorkingSet64;

			}
			catch (Win32Exception e)
			{
				return;
			}
		}

		public double GetCpu(int pid)
		{
			return 0;
		}

		public long GetMemory(int pid)
		{
			return 0;
		}
	}
}
