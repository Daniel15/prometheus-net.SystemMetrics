using System.Runtime.InteropServices;

namespace Prometheus.SystemMetrics.Native
{
	public class WindowsNative
	{
		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public class MEMORYSTATUSEX
	{
		public uint dwLength;
		public uint dwMemoryLoad;
		public ulong ullTotalPhys;
		public ulong ullAvailPhys;
		public ulong ullTotalPageFile;
		public ulong ullAvailPageFile;
		public ulong ullTotalVirtual;
		public ulong ullAvailVirtual;
		public ulong ullAvailExtendedVirtual;
		public MEMORYSTATUSEX()
		{
			dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
		}
	}
}
