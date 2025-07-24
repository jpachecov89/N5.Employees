using MediatR;
using Microsoft.Extensions.Logging;
using N5.Employees.Application.Common.Models;
using N5.Employees.Application.Interfaces.ElasticSearch;
using N5.Employees.Application.Interfaces.Kafka;
using System.Text.Json;

namespace N5.Employees.Application.Permissions.Queries.GetPermissions
{
	public class GetPermissionsHandler : IRequestHandler<GetPermissionsQuery, List<GetPermissionsDto>>
	{
		private readonly ILogger<GetPermissionsHandler> _logger;
		private readonly IKafkaProducerSerivce _kafkaProducer;
		private readonly IElasticSearchService _elastic;

		public GetPermissionsHandler(
			ILogger<GetPermissionsHandler> logger,
			IKafkaProducerSerivce kafkaProducer,
			IElasticSearchService elastic)
		{
			_logger = logger;
			_kafkaProducer = kafkaProducer;
			_elastic = elastic;
		}

		public async Task<List<GetPermissionsDto>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
		{
			_logger.LogInformation($"Executing GetPermissions with filters: {request}");
			_logger.LogInformation("Publishing get operation to Kafka");

			await _kafkaProducer.PublishAsync(new OperationLogDto
			{
				Id = Guid.NewGuid(),
				Operation = "get",
				Payload = JsonSerializer.Serialize(new
				{
					Id = Guid.NewGuid(),
					request.PermissionTypeId,
					request.PermissionTypeName,
					request.EmployeeId,
					request.EmployeeName,
					request.FromDate,
					request.ToDate
				})
			});

			var result = await _elastic.SearchAll(request);

			_logger.LogInformation($"Retrieved {result.Count} permissions from ElasticSearch");

			return result;
		}
	}
}