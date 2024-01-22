using System.Runtime.InteropServices;

namespace Prometheus.SystemMetrics.Native
{
	/// <summary>
	/// Native API calls for Linux.
	/// </summary>
	internal static partial class LinuxNative
	{
		public const int SC_CLK_TCK = 0x2;

#if NET7_0_OR_GREATER
		[LibraryImport("libc", SetLastError = true)]
		[DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
		internal static partial int getloadavg(double[] loadavg, int nelem);

		[LibraryImport("libc", SetLastError = true)]
		[DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
		internal static partial long sysconf(int name);
#else
		[DllImport("libc", SetLastError = true)]
		[DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
		internal static extern int getloadavg(double[] loadavg, int nelem);

		[DllImport("libc", SetLastError = true)]
		[DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
		internal static extern long sysconf(int name);
#endif
	}
}
