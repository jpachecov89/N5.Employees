using MediatR;

namespace N5.Employees.Application.Permissions.Commands.UpdatePermission
{
	public record UpdatePermissionCommand(
		Guid PermissionId,
		Guid EmployeeId,
		Guid PermissionTypeId,
		DateTime DateGranted
	) : IRequest<UpdatePermissionResult>;
}