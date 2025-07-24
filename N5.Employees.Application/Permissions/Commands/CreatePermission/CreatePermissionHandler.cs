using MediatR;
using Microsoft.Extensions.Logging;
using N5.Employees.Application.Common.Models;
using N5.Employees.Application.Exceptions;
using N5.Employees.Application.Interfaces.Kafka;
using N5.Employees.Application.Interfaces.Persistence;
using N5.Employees.Domain.Entities;
using System.Text.Json;

namespace N5.Employees.Application.Permissions.Commands.CreatePermission
{
	public class CreatePermissionHandler : IRequestHandler<CreatePermissionCommand, CreatePermissionResult>
	{
		private readonly ILogger<CreatePermissionHandler> _logger;
		private readonly IUnitOfWork _uow;
		private readonly IKafkaProducerSerivce _kafkaProducer;

		public CreatePermissionHandler(
			ILogger<CreatePermissionHandler> logger,
			IUnitOfWork uow,
			IKafkaProducerSerivce kafkaProducer)
		{
			_logger = logger;
			_uow = uow;
			_kafkaProducer = kafkaProducer;
		}

		public async Task<CreatePermissionResult> Handle(CreatePermissionCommand request, CancellationToken cancellationToken)
		{
			_logger.LogInformation($"Executing CreatePermission for EmployeeId: {request.EmployeeId}, PermissionTypeId: {request.PermissionTypeId}, DateGranted: {request.DateGranted}");

			var permissionType = await _uow.PermissionTypes.GetByIdAsync(request.PermissionTypeId)
				?? throw new NotFoundException(nameof(PermissionType), request.PermissionTypeId);
			
			var employee = await _uow.Employees.GetByIdAsync(request.EmployeeId)
				?? throw new NotFoundException(nameof(Employee), request.EmployeeId);
			
			var permission = new Permission
			(
				request.EmployeeId,
				request.PermissionTypeId,
				request.DateGranted
			);

			await _uow.Permissions.AddAsync(permission);
			await _uow.SaveChangesAsync(cancellationToken);

			_logger.LogInformation($"Permission created with Id: {permission.Id}");
			_logger.LogInformation("Publishing message to Kafka with operation: request");

			await _kafkaProducer.PublishAsync(new OperationLogDto
			{
				Id = Guid.NewGuid(),
				Operation = "request",
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

			return new CreatePermissionResult
			(
				permission.Id,
				employee.FullName,
				permissionType.Description,
				permission.DateGranted
			);
		}
	}
}