using System.Runtime.InteropServices;

namespace Prometheus.SystemMetrics
{
	/// <summary>
	/// Native API calls for Linux.
	/// </summary>
	internal static class LinuxNative
	{
		[DllImport("libc", SetLastError = true)]
		internal static extern int getloadavg(double[] loadavg, int nelem);
	}
}
