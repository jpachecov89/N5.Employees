using MediatR;

namespace N5.Employees.Application.Permissions.Commands.CreatePermission
{
	public record CreatePermissionCommand(
		Guid EmployeeId,
		Guid PermissionTypeId,
		DateTime DateGranted
	) : IRequest<CreatePermissionResult>;
}