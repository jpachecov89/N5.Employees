namespace N5.Employees.Application.Permissions.Commands.CreatePermission
{
	public record CreatePermissionResult(
		Guid PermissionId,
		string EmployeeFullName,
		string PermissionTypeName,
		DateTime DateGranted
	);
}