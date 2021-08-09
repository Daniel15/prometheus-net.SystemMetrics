using System.Runtime.InteropServices;

namespace Prometheus.SystemMetrics.Native
{
	/// <summary>
	/// Native API calls for Windows.
	/// </summary>
	internal static class WindowsNative
	{
		/// <summary>
		/// Retrieves information about the system's current usage of both physical and virtual memory.
		/// See https://docs.microsoft.com/en-us/windows/win32/api/sysinfoapi/nf-sysinfoapi-globalmemorystatusex
		/// </summary>
		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);
	}

	/// <summary>
	/// Contains information about the current state of both physical and virtual memory, including extended memory.
	/// See https://docs.microsoft.com/en-us/windows/win32/api/sysinfoapi/ns-sysinfoapi-memorystatusex
	/// </summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public class MEMORYSTATUSEX
	{
		/// <summary>
		/// The size of the structure, in bytes. You must set this member before calling 
		/// GlobalMemoryStatusEx.
		/// </summary>
		public uint dwLength;

		/// <summary>
		/// A number between 0 and 100 that specifies the approximate percentage of physical memory
		/// that is in use (0 indicates no memory use and 100 indicates full memory use).
		/// </summary>
		public uint dwMemoryLoad;

		/// <summary>
		/// The amount of actual physical memory, in bytes.
		/// </summary>
		public ulong ullTotalPhys;

		/// <summary>
		/// The amount of physical memory currently available, in bytes. This is the amount of physical
		/// memory that can be immediately reused without having to write its contents to disk first.
		/// It is the sum of the size of the standby, free, and zero lists.
		/// </summary>
		public ulong ullAvailPhys;

		/// <summary>
		/// The current committed memory limit for the system or the current process, whichever is
		/// smaller, in bytes. To get the system-wide committed memory limit, call GetPerformanceInfo.
		/// </summary>
		public ulong ullTotalPageFile;

		/// <summary>
		/// The maximum amount of memory the current process can commit, in bytes. This value is
		/// equal to or smaller than the system-wide available commit value. To calculate the
		/// system-wide available commit value, call GetPerformanceInfo and subtract the value
		/// of CommitTotal from the value of CommitLimit.
		/// </summary>
		public ulong ullAvailPageFile;

		/// <summary>
		/// The size of the user-mode portion of the virtual address space of the calling process,
		/// in bytes. This value depends on the type of process, the type of processor, and the
		/// configuration of the operating system. For example, this value is approximately 2 GB
		/// for most 32-bit processes on an x86 processor and approximately 3 GB for 32-bit
		/// processes that are large address aware running on a system with 4-gigabyte tuning
		/// enabled.
		/// </summary>
		public ulong ullTotalVirtual;

		/// <summary>
		/// The amount of unreserved and uncommitted memory currently in the user-mode portion
		/// of the virtual address space of the calling process, in bytes.
		/// </summary>
		public ulong ullAvailVirtual;

		/// <summary>
		/// Reserved. This value is always 0.
		/// </summary>
		public ulong ullAvailExtendedVirtual;

		/// <summary>
		/// Creates a new instance of <see cref="MEMORYSTATUSEX"/>.
		/// </summary>
		public MEMORYSTATUSEX()
		{
			dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
		}
	}
}
