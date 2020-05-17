using System.Linq;
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

			var result = ProcStatParser.ParseCpuUsage(input, CLOCK_TICKS_PER_SECOND).ToList();

			using (new AssertionScope())
			{
				result.Should().HaveCount(1);
				var data = result[0];
				data.CpuIndex.Should().Be(0);
				data.Idle.Should().Be(2374070.16);
				data.IoWait.Should().Be(85611.95);
				data.Irq.Should().Be(0);
				data.Nice.Should().Be(4756.44);
				data.SoftIrq.Should().Be(7909.19);
				data.Steal.Should().Be(14336.02);
				data.System.Should().Be(109058.75);
				data.User.Should().Be(257973.56);
			}
		}

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

			var result = ProcStatParser.ParseCpuUsage(input, CLOCK_TICKS_PER_SECOND).ToList();

			result.Should().HaveCount(4);

			using (new AssertionScope())
			{
				result[0].CpuIndex.Should().Be(0);
				result[0].Idle.Should().Be(907306.09);
				result[0].IoWait.Should().Be(930.13);
				result[0].Irq.Should().Be(0);
				result[0].Nice.Should().Be(596.02);
				result[0].SoftIrq.Should().Be(274.43);
				result[0].Steal.Should().Be(13707.66);
				result[0].System.Should().Be(10471.64);
				result[0].User.Should().Be(27131.21);
			}

			using (new AssertionScope())
			{
				result[1].CpuIndex.Should().Be(1);
				result[1].Idle.Should().Be(919501.47);
				result[1].IoWait.Should().Be(1141.7);
				result[1].Irq.Should().Be(0);
				result[1].Nice.Should().Be(678.85);
				result[1].SoftIrq.Should().Be(425.02);
				result[1].Steal.Should().Be(18018.92);
				result[1].System.Should().Be(9217.79);
				result[1].User.Should().Be(13871.86);
			}

			using (new AssertionScope())
			{
				result[2].CpuIndex.Should().Be(2);
				result[2].Idle.Should().Be(920388.64);
				result[2].IoWait.Should().Be(1106.48);
				result[2].Irq.Should().Be(0);
				result[2].Nice.Should().Be(711.15);
				result[2].SoftIrq.Should().Be(182.55);
				result[2].Steal.Should().Be(17366.43);
				result[2].System.Should().Be(9430.43);
				result[2].User.Should().Be(14144.61);
			}

			using (new AssertionScope())
			{
				result[3].CpuIndex.Should().Be(3);
				result[3].Idle.Should().Be(924104.18);
				result[3].IoWait.Should().Be(894.9);
				result[3].Irq.Should().Be(0);
				result[3].Nice.Should().Be(675.08);
				result[3].SoftIrq.Should().Be(93.13);
				result[3].Steal.Should().Be(14541.28);
				result[3].System.Should().Be(7310.65);
				result[3].User.Should().Be(13414.1);
			}
		}
	}
}
