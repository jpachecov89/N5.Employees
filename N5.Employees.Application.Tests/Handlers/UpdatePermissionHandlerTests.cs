using Microsoft.Extensions.Logging;
using Moq;
using N5.Employees.Application.Common.Models;
using N5.Employees.Application.Exceptions;
using N5.Employees.Application.Interfaces.Kafka;
using N5.Employees.Application.Interfaces.Persistence;
using N5.Employees.Application.Permissions.Commands.UpdatePermission;
using N5.Employees.Domain.Entities;

namespace N5.Employees.Application.Tests.Handlers
{
	public class UpdatePermissionHandlerTests
	{
		private readonly Mock<ILogger<UpdatePermissionHandler>> _loggerMock = new();
		private readonly Mock<IUnitOfWork> _uowMock = new();
		private readonly Mock<IKafkaProducerSerivce> _kafkaMock = new();

		private readonly UpdatePermissionHandler _handler;

		public UpdatePermissionHandlerTests()
		{
			_handler = new UpdatePermissionHandler(_loggerMock.Object, _uowMock.Object, _kafkaMock.Object);
		}

		[Fact]
		public async Task Handle_ShouldUpdatePermissionAndPublishToKafka()
		{
			var permissionId = Guid.Empty;
			var employeeId = Guid.NewGuid();
			var permissionTypeId = Guid.NewGuid();
			var date = DateTime.UtcNow;

			var employee = new Employee { Id = employeeId, FirstName = "John", LastName = "Doe" };
			var permissionType = new PermissionType { Id = permissionTypeId, Description = "Remote Work" };

			var permission = new Permission
			(
				Guid.NewGuid(),
				Guid.NewGuid(),
				DateTime.UtcNow.AddDays(-1)
			)
			{
				PermissionType = permissionType,
				Employee = employee
			};

			permissionId = permission.Id;

			_uowMock.Setup(x => x.Permissions.GetByIdAsync(permissionId))
					.ReturnsAsync(permission);

			_uowMock.Setup(x => x.Employees.GetByIdAsync(employeeId))
					.ReturnsAsync(employee);

			_uowMock.Setup(x => x.PermissionTypes.GetByIdAsync(permissionTypeId))
					.ReturnsAsync(permissionType);

			var command = new UpdatePermissionCommand(
				permissionId,
				employeeId,
				permissionTypeId,
				date
			);

			var result = await _handler.Handle(command, CancellationToken.None);

			_uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
			_kafkaMock.Verify(x => x.PublishAsync(It.IsAny<OperationLogDto>()), Times.Once);

			Assert.Equal(permissionId, result.PermissionId);
			Assert.Equal(employee.FullName, result.EmployeeFullName);
			Assert.Equal(permissionType.Description, result.PermissionTypeName);
			Assert.Equal(date, result.DateGranted);

			Assert.Equal(employeeId, permission.EmployeeId);
			Assert.Equal(permissionTypeId, permission.PermissionTypeId);
			Assert.Equal(date, permission.DateGranted);
		}

		[Fact]
		public async Task Handle_ShouldThrowNotFoundException_WhenPermissionDoesNotExist()
		{
			var command = new UpdatePermissionCommand(
				Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow);

			_uowMock.Setup(x => x.Permissions.GetByIdAsync(command.PermissionId))
				.ReturnsAsync((Permission)null);

			await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
		}

		[Fact]
		public async Task Handle_ShouldThrowNotFoundException_WhenEmployeeDoesNotExist()
		{
			var permissionId = Guid.Empty;
			var employeeId = Guid.NewGuid();
			var permissionTypeId = Guid.NewGuid();

			var permission = new Permission(employeeId, permissionTypeId, DateTime.UtcNow);
			permissionId = permission.Id;

			_uowMock.Setup(x => x.Permissions.GetByIdAsync(permissionId))
				.ReturnsAsync(permission);

			_uowMock.Setup(x => x.Employees.GetByIdAsync(employeeId))
				.ReturnsAsync((Employee)null);

			var command = new UpdatePermissionCommand(permissionId, employeeId, permissionTypeId, DateTime.UtcNow);

			await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
		}

		[Fact]
		public async Task Handle_ShouldThrowNotFoundException_WhenPermissionTypeDoesNotExist()
		{
			var permissionId = Guid.Empty;
			var employeeId = Guid.NewGuid();
			var permissionTypeId = Guid.NewGuid();

			var permission = new Permission(employeeId, permissionTypeId, DateTime.UtcNow);
			permissionId = permission.Id;

			var employee = new Employee { Id = employeeId };

			_uowMock.Setup(x => x.Permissions.GetByIdAsync(permissionId))
				.ReturnsAsync(permission);

			_uowMock.Setup(x => x.Employees.GetByIdAsync(employeeId))
				.ReturnsAsync(employee);

			_uowMock.Setup(x => x.PermissionTypes.GetByIdAsync(permissionTypeId))
				.ReturnsAsync((PermissionType)null);

			var command = new UpdatePermissionCommand(permissionId, employeeId, permissionTypeId, DateTime.UtcNow);

			await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
		}
	}
}