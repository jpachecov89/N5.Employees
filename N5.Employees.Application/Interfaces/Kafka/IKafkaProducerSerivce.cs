using N5.Employees.Application.Common.Models;

namespace N5.Employees.Application.Interfaces.Kafka
{
	public interface IKafkaProducerSerivce
	{
		Task PublishAsync(OperationLogDto message);
	}
}