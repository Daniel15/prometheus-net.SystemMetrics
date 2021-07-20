using System.Collections.Generic;

namespace Prometheus.SystemMetrics.Helper
{
	public class MemInfoHelper
	{
		public static readonly Dictionary<string, string> Labels = new Dictionary<string, string>
		{
			//Common
			{ "MEMTOTAL", "MemoryTotal" },
			{ "ULLTOTALPHYS", "MemoryTotal" },
			{ "ULLAVAILPHYS", "MemoryFree"},
			{ "MEMFREE", "MemoryFree"},
			{ "MEMUSEDCOMPONENT", "MemoryUsedComponent" },

			//Linux
			{ "BUFFERS", "unixBuffers" },
			{ "CACHED", "unixCaches" },
			{ "ACTIVE(ANON)", "unixActiveAnon" },
			{ "INACTIVE(ANON)", "unixInactiveAnon" },
			{ "ACTIVE(FILE)", "unixActiveFile" },
			{ "DIRTY", "unixDirty" },

			//Windows
			{ "DWMEMORYLOAD", "windowsMemoryLoad" },
			{ "ULLTOTALPAGEFILE", "windowsTotalPageFile" },
			{ "ULLAVAILPAGEFILE", "windowsAvailablePageFile" },
			{ "ULLTOTALVIRTUAL", "windowsTotalVirtual" },
			{ "ULLAVAILVIRTUAL", "windowsAvailableVirtual" },
			{ "ULLAVAILEXTENDEDVIRTUAL", "windowsAvailableExtendedVirtual" }
		};
	}
}
