using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Prometheus.SystemMetrics.Parsers;
using Xunit;

namespace Prometheus.SystemMetrics.Tests.Parsers
{
	public class MemInfoTest
	{
		private const int BYTES_IN_KB = 1024;

		[Fact]
		public void ParsesMemInfo()
		{
			const string input = @"
MemTotal:       16470140 kB
MemFree:         3690788 kB
Buffers:           34032 kB
Cached:           188576 kB
Active(anon):     103104 kB
Inactive(anon):    17440 kB
Active(file):      64452 kB
Dirty:                 0 kB
HugePages_Total:       0
";
			var result = MemInfoParser.Parse(input);
			result.Should().BeEquivalentTo(new[]
			{
				("MemTotal", 16470140L * BYTES_IN_KB),
				("MemFree", 3690788L * BYTES_IN_KB),
				("Buffers", 34032L * BYTES_IN_KB),
				("Cached", 188576L * BYTES_IN_KB),
				("Active(anon)", 103104L * BYTES_IN_KB),
				("Inactive(anon)", 17440L * BYTES_IN_KB),
				("Active(file)", 64452L * BYTES_IN_KB),
				("Dirty", 0),
			});
		}
	}
}
