using Microsoft.Extensions.Logging;
using Moq;
using N5.Employees.Application.Common.Models;
using N5.Employees.Application.Exceptions;
using N5.Employees.Application.Interfaces.Kafka;
using N5.Employees.Application.Interfaces.Persistence;
using N5.Employees.Application.Permissions.Commands.CreatePermission;
using N5.Employees.Domain.Entities;

namespace N5.Employees.Application.Tests.Handlers
{
	public class CreatePermissionHandlerTests
	{
		private readonly Mock<ILogger<CreatePermissionHandler>> _loggerMock = new();
		private readonly Mock<IUnitOfWork> _uowMock = new();
		private readonly Mock<IKafkaProducerSerivce> _kafkaMock = new();

		private readonly CreatePermissionHandler _handler;

		public CreatePermissionHandlerTests()
		{
			_handler = new CreatePermissionHandler(_loggerMock.Object, _uowMock.Object, _kafkaMock.Object);
		}

		[Fact]
		public async Task Handle_ShouldCreatePermissionAndPublishToKafka()
		{
			var employeeId = Guid.NewGuid();
			var permissionTypeId = Guid.NewGuid();
			var date = DateTime.UtcNow;

			var employee = new Employee { Id = employeeId, FirstName = "John", LastName = "Doe" };
			var permissionType = new PermissionType { Id = permissionTypeId, Description = "Paid Vacations" };

			_uowMock.Setup(x => x.Employees.GetByIdAsync(employeeId))
				.ReturnsAsync(employee);

			_uowMock.Setup(x => x.PermissionTypes.GetByIdAsync(permissionTypeId))
				.ReturnsAsync(permissionType);

			var permissionsRepoMock = new Mock<IPermissionRepository>();
			permissionsRepoMock
				.Setup(x => x.AddAsync(It.IsAny<Permission>()))
				.Callback<Permission>(p =>
				{
					p.Id = Guid.NewGuid();
					p.PermissionType = permissionType;
					p.Employee = employee;
				});
			_uowMock.Setup(x => x.Permissions).Returns(permissionsRepoMock.Object);

			var createCommand = new CreatePermissionCommand
			(
				employeeId,
				permissionTypeId,
				date
			);

			var result = await _handler.Handle(createCommand, CancellationToken.None);

			permissionsRepoMock.Verify(x => x.AddAsync(It.Is<Permission>(p =>
				p.EmployeeId == employeeId &&
				p.PermissionTypeId == permissionTypeId &&
				p.DateGranted == date)), Times.Once);

			_uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

			_kafkaMock.Verify(x => x.PublishAsync(It.IsAny<OperationLogDto>()), Times.Once);

			Assert.Equal(employee.FullName, result.EmployeeFullName);
			Assert.Equal(permissionType.Description, result.PermissionTypeName);
			Assert.Equal(date, result.DateGranted);
		}

		[Fact]
		public async Task Handle_ShouldThrowNotFoundException_WhenEmployeeDoesNotExist()
		{
			var employeeId = Guid.NewGuid();
			var permissionTypeId = Guid.NewGuid();

			_uowMock.Setup(x => x.Employees.GetByIdAsync(employeeId)).ReturnsAsync((Employee)null);
			_uowMock.Setup(x => x.PermissionTypes.GetByIdAsync(It.IsAny<Guid>()))
					.ReturnsAsync(new PermissionType { Id = permissionTypeId, Description = "Vacaciones" });

			var command = new CreatePermissionCommand
			(
				employeeId,
				permissionTypeId,
				DateTime.UtcNow
			);

			await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
		}

		[Fact]
		public async Task Handle_ShouldThrowNotFoundException_WhenPermissionTypeDoesNotExist()
		{
			var employeeId = Guid.NewGuid();
			var permissionTypeId = Guid.NewGuid();

			_uowMock.Setup(x => x.Employees.GetByIdAsync(employeeId))
					.ReturnsAsync(new Employee { Id = employeeId, FirstName = "John", LastName = "Doe" });

			_uowMock.Setup(x => x.PermissionTypes.GetByIdAsync(permissionTypeId))
					.ReturnsAsync((PermissionType)null);

			var command = new CreatePermissionCommand
			(
				employeeId,
				permissionTypeId,
				DateTime.UtcNow
			);

			await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
		}
	}
}