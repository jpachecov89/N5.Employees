using N5.Employees.Application.Common.Models;
using N5.Employees.Application.Interfaces.Kafka;

namespace N5.Employees.Application.IntegrationTests.Helpers
{
	public class FakeKafkaProducer : IKafkaProducerSerivce
	{
		public Task PublishAsync(OperationLogDto message)
		{
			return Task.CompletedTask;
		}
	}
}