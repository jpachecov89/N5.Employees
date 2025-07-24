namespace N5.Employees.Infrastructure.Kafka.Configurations
{
	public class KafkaSettings
	{
		public string BootstrapServers { get; set; } = null!;
		public string Topic { get; set; } = null!;
	}
}