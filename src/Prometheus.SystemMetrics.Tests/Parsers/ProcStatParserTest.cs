using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Execution;
using Prometheus.SystemMetrics.Parsers;
using Xunit;

namespace Prometheus.SystemMetrics.Tests.Parsers
{
	public class ProcStatParserTest
	{
		private const int CLOCK_TICKS_PER_SECOND = 100;

		[Fact]
		public void ParsesProcStatOneCpu()
		{
			const string input = @"
cpu  25797356 475644 10905875 237407016 8561195 0 790919 1433602 0 0
cpu0 25797356 475644 10905875 237407016 8561195 0 790919 1433602 0 0
intr 2102264361 2 9 0 0 0 0 3 0 1 0 0 33 15 0 0 2863339 0 0 0 0 0 0 0 0 0 142080017 0 187462829 141643800 0 0 0 0 0 0 2766816 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
ctxt 5373393714
btime 1586466311
processes 23736296
procs_running 1
procs_blocked 0
softirq 1500396835 0 271650897 70556582 475462237 143674570 0 54787062 0 6656 484258831
";

			var result = ProcStatParser.ParseCpuUsage(input, CLOCK_TICKS_PER_SECOND);

			using (new AssertionScope())
			{
				result.Should().HaveCount(1);
				result["0"].Should().BeEquivalentTo(new ProcStatParser.CpuUsageData
				{
					User = 257973.56,
					Nice = 4756.44,
					System = 109058.75,
					Idle = 2374070.16,
					IoWait = 85611.95,
					Irq = 0,
					SoftIrq = 7909.19,
					Steal = 14336.02,
				});
			}
		}

		// TODO
		[Fact]
		public void ParsesProcStatMultipleCpus()
		{
			const string input = @"
cpu  6856179 266112 3643053 367130039 407323 0 97515 6363431 0 0
cpu0 2713121 59602 1047164 90730609 93013 0 27443 1370766 0 0
cpu1 1387186 67885 921779 91950147 114170 0 42502 1801892 0 0
cpu2 1414461 71115 943043 92038864 110648 0 18255 1736643 0 0
cpu3 1341410 67508 731065 92410418 89490 0 9313 1454128 0 0
intr 304984032 3 9 0 0 0 0 0 0 0 0 0 0 3 0 0 942829 0 0 0 0 0 0 0 0 0 1636816 0 13318452 7577792 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
ctxt 599038287
btime 1588487095
processes 636829
procs_running 3
procs_blocked 0
softirq 214206550 0 68470359 13 31547001 2150475 0 1007474 41089356 794 69941078
";

			var result = ProcStatParser.ParseCpuUsage(input, CLOCK_TICKS_PER_SECOND);

			using (new AssertionScope())
			{
				result.Should().HaveCount(4);
				result.Should().BeEquivalentTo(new Dictionary<string, ProcStatParser.CpuUsageData>
				{
					{"0", new ProcStatParser.CpuUsageData
					{
						Idle = 907306.09,
						IoWait = 930.13,
						Irq = 0.0,
						Nice = 596.02,
						SoftIrq = 274.43,
						Steal = 13707.66,
						System = 10471.64,
						User = 27131.21,
					}},
					{"1", new ProcStatParser.CpuUsageData
					{
						Idle = 919501.47,
						IoWait = 1141.7,
						Irq = 0.0,
						Nice = 678.85,
						SoftIrq = 425.02,
						Steal = 18018.92,
						System = 9217.79,
						User = 13871.86,
					}},
					{"2", new ProcStatParser.CpuUsageData
					{
						Idle = 920388.64,
						IoWait = 1106.48,
						Irq = 0.0,
						Nice = 711.15,
						SoftIrq = 182.55,
						Steal = 17366.43,
						System = 9430.43,
						User = 14144.61,
					}},
					{"3", new ProcStatParser.CpuUsageData
					{
						Idle = 924104.18,
						IoWait = 894.9,
						Irq = 0.0,
						Nice = 675.08,
						SoftIrq = 93.13,
						Steal = 14541.28,
						System = 7310.65,
						User = 13414.1,
					}},
				});
			}
		}
	}
}
