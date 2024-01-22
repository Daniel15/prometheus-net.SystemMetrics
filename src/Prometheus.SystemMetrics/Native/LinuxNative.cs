using System.Runtime.InteropServices;

namespace Prometheus.SystemMetrics.Native
{
	/// <summary>
	/// Native API calls for Linux.
	/// </summary>
	internal static class LinuxNative
	{
		public const int SC_CLK_TCK = 0x2;

		[DllImport("libc", SetLastError = true)]
		[DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
		internal static extern int getloadavg(double[] loadavg, int nelem);

		[DllImport("libc", SetLastError = true)]
		[DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
		public static extern long sysconf(int name);
	}
}
