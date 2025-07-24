using Confluent.Kafka;
using Microsoft.Extensions.Options;
using N5.Employees.Application.Common.Models;
using N5.Employees.Application.Interfaces.Kafka;
using N5.Employees.Infrastructure.Kafka.Configurations;
using System.Text.Json;

namespace N5.Employees.Infrastructure.Kafka
{
	public class KafkaProducerService : IKafkaProducerSerivce
	{
		private readonly IProducer<Null, string> _producer;
		private readonly string _topic;
		public KafkaProducerService(IOptions<KafkaSettings> kafkaSettings)
		{
			var config = new ProducerConfig
			{
				BootstrapServers = kafkaSettings.Value.BootstrapServers
			};

			_topic = kafkaSettings.Value.Topic;
			_producer = new ProducerBuilder<Null, string>(config).Build();
		}

		public async Task PublishAsync(OperationLogDto message)
		{
			var json = JsonSerializer.Serialize(message);
			await _producer.ProduceAsync(_topic, new Message<Null, string> { Value = json } );
		}
	}
}