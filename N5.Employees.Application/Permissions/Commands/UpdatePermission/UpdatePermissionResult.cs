namespace N5.Employees.Application.Permissions.Commands.UpdatePermission
{
	public record UpdatePermissionResult(
		Guid PermissionId,
		string EmployeeFullName,
		string PermissionTypeName,
		DateTime DateGranted
	);
}