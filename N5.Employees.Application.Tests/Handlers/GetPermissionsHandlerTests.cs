using Microsoft.Extensions.Logging;
using Moq;
using N5.Employees.Application.Common.Models;
using N5.Employees.Application.Interfaces.ElasticSearch;
using N5.Employees.Application.Interfaces.Kafka;
using N5.Employees.Application.Permissions.Queries.GetPermissions;

namespace N5.Employees.Application.Tests.Handlers
{
	public class GetPermissionsHandlerTests
	{
		private readonly Mock<ILogger<GetPermissionsHandler>> _loggerMock = new();
		private readonly Mock<IKafkaProducerSerivce> _kafkaMock = new();
		private readonly Mock<IElasticSearchService> _elasticMock = new();

		private readonly GetPermissionsHandler _handler;

		public GetPermissionsHandlerTests()
		{
			_handler = new GetPermissionsHandler(_loggerMock.Object, _kafkaMock.Object, _elasticMock.Object);
		}

		[Fact]
		public async Task Handle_ShouldPublishToKafka_And_ReturnResultsFromElastic()
		{
			var query = new GetPermissionsQuery
			(
				Guid.NewGuid(),
				"John",
				Guid.NewGuid(),
				"Vacations",
				DateTime.UtcNow.AddDays(-30),
				DateTime.UtcNow
			);

			var expectedResult = new List<GetPermissionsDto>
			{
				new(Guid.NewGuid(), "John Doe", "Paid Vacations", DateTime.UtcNow)
			};

			_elasticMock.Setup(x => x.SearchAll(query))
				.ReturnsAsync(expectedResult);

			var result = await _handler.Handle(query, CancellationToken.None);

			_kafkaMock.Verify(x => x.PublishAsync(It.IsAny<OperationLogDto>()), Times.Once);
			_elasticMock.Verify(x => x.SearchAll(query), Times.Once);

			Assert.Single(result);
			Assert.Equal("John Doe", result[0].EmployeeFullName);
			Assert.Equal("Paid Vacations", result[0].PermissionTypeName);
		}
	}
}