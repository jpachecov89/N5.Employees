using MediatR;
using Microsoft.Extensions.Logging;
using N5.Employees.Application.Common.Models;
using N5.Employees.Application.Exceptions;
using N5.Employees.Application.Interfaces.Kafka;
using N5.Employees.Application.Interfaces.Persistence;
using N5.Employees.Domain.Entities;
using System.Text.Json;

namespace N5.Employees.Application.Permissions.Commands.UpdatePermission
{
	public class UpdatePermissionHandler : IRequestHandler<UpdatePermissionCommand, UpdatePermissionResult>
	{
		private readonly ILogger<UpdatePermissionHandler> _logger;
		private readonly IUnitOfWork _uow;
		private readonly IKafkaProducerSerivce _kafkaProducer;

		public UpdatePermissionHandler(
			ILogger<UpdatePermissionHandler> logger,
			IUnitOfWork uow,
			IKafkaProducerSerivce kafkaProducer)
		{
			_logger = logger;
			_uow = uow;
			_kafkaProducer = kafkaProducer;
		}

		public async Task<UpdatePermissionResult> Handle(UpdatePermissionCommand request, CancellationToken cancellationToken)
		{
			_logger.LogInformation($"Executing UpdatePermission for PermissionId: {request.PermissionId} with new values: EmployeeId={request.EmployeeId}, PermissionTypeId={request.PermissionTypeId}, DateGranted={request.DateGranted}");

			var permission = await _uow.Permissions.GetByIdAsync(request.PermissionId)
				?? throw new NotFoundException(nameof(Permission), request.PermissionId);

			var employee = await _uow.Employees.GetByIdAsync(request.EmployeeId)
				?? throw new NotFoundException(nameof(Employee), request.EmployeeId);

			var permissionType = await _uow.PermissionTypes.GetByIdAsync(request.PermissionTypeId)
				?? throw new NotFoundException(nameof(PermissionType), request.PermissionTypeId);

			permission.PermissionTypeId = request.PermissionTypeId;
			permission.EmployeeId = request.EmployeeId;
			permission.DateGranted = request.DateGranted;

			await _uow.SaveChangesAsync(cancellationToken);

			_logger.LogInformation($"Permission with Id: {permission.Id} updated successfully");
			_logger.LogInformation("Publishing update event to Kafka with operation: modify");

			await _kafkaProducer.PublishAsync(new OperationLogDto
			{
				Id = Guid.NewGuid(),
				Operation = "modify",
				Payload = JsonSerializer.Serialize(new
				{
					Id = permission.Id.ToString(),
					PermissionTypeId = permission.PermissionTypeId.ToString(),
					PermissionTypeName = permission.PermissionType.Description,
					EmployeeId = permission.EmployeeId.ToString(),
					EmployeeName = permission.Employee.FullName,
					DateGranted = permission.DateGranted
				})
			});

			_logger.LogInformation($"Kafka publish completed for PermissionId: {permission.Id}");

			return new UpdatePermissionResult(
				permission.Id,
				employee.FullName,
				permissionType.Description,
				permission.DateGranted
			);
		}
	}
}