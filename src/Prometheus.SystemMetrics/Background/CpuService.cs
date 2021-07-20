using System;
using System.Collections.Concurrent;
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
		public static double CpuUsed { get; set; }
		public static long MemoryUsed { get; set; }
		private static ConcurrentDictionary<int, CpuInfo> LastUpdate { get; } = new ConcurrentDictionary<int, CpuInfo>();

		public async Task RunUpdateAsync(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				var processes = Process.GetProcesses();
				double cpu = 0;
				long memory = 0;
				foreach (var process in processes)
				{
					var info = Calc(process);

					if (info == null)
						continue;

					cpu += info.Cpu;
					memory += info.Memory;
				}

				CpuUsed = cpu;
				MemoryUsed = memory;

				await Task.Delay(1000, cancellationToken);
			}
		}

		public ProcessInfo Calc(Process proc)
		{
			try
			{
				if (proc.Id == 0)
					return null;

				if (proc.HasExited)
					return null;

				if (LastUpdate.TryAdd(proc.Id, new CpuInfo()
				{
					LastProcTime = proc.TotalProcessorTime,
					LastUpdate = DateTime.UtcNow,
					MemorySize = proc.WorkingSet64
				}))
					return null;

				var oldTime = LastUpdate[proc.Id];
				var curDateTime = DateTime.UtcNow;
				var interval = curDateTime - oldTime.LastUpdate;
				var totalProcTime = proc.TotalProcessorTime;
				var curTime = totalProcTime - oldTime.LastProcTime;
				var cpu = ((curTime.TotalMilliseconds / interval.TotalMilliseconds) / Environment.ProcessorCount) * 100;

				oldTime.LastProcTime = totalProcTime;
				oldTime.LastUpdate = curDateTime;
				oldTime.LastPercent = cpu;
				oldTime.MemorySize = proc.WorkingSet64;

				return new ProcessInfo()
				{
					Cpu = cpu,
					Memory = oldTime.MemorySize
				};
			}
			catch (Win32Exception e)
			{
				return null;
			}
		}

		public double GetCpu(int pid)
		{
			if (!LastUpdate.ContainsKey(pid))
				return 0;

			return LastUpdate[pid].LastPercent;
		}

		public long GetMemory(int pid)
		{
			if (!LastUpdate.ContainsKey(pid))
				return 0;

			return LastUpdate[pid].MemorySize;
		}
	}
}
