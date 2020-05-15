using System.Net.NetworkInformation;

namespace Prometheus.SystemMetrics.Collectors
{
	/// <summary>
	/// Collects data on network usage
	/// </summary>
	public class NetworkCollector : ISystemMetricCollector
	{
		internal Counter NetworkReceived { get; private set; } = default!;
		internal Counter NetworkSent { get; private set; } = default!;

		/// <summary>
		/// Gets whether this metric is supported on the current system.
		/// </summary>
		public bool IsSupported => true;

		/// <summary>
		/// Creates the Prometheus metric.
		/// </summary>
		/// <param name="factory">Factory to create metric using</param>
		public void CreateMetrics(MetricFactory factory)
		{
			NetworkReceived = factory.CreateCounter(
				"node_network_receive_bytes_total",
				"Bytes received over the network",
				"device"
			);
			NetworkSent = factory.CreateCounter(
				"node_network_transmit_bytes_total",
				"Bytes sent over the network", 
				"device"
			);
		}

		/// <summary>
		/// Update data for the metrics. Called immediately before the metrics are scraped.
		/// </summary>
		public void UpdateMetrics()
		{
			foreach (var iface in NetworkInterface.GetAllNetworkInterfaces())
			{
				var stats = iface.GetIPStatistics();
				NetworkReceived.WithLabels(iface.Name).IncTo(stats.BytesReceived);
				NetworkSent.WithLabels(iface.Name).IncTo(stats.BytesSent);
			}
		}
	}
}
