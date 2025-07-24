namespace N5.Employees.Application.Permissions.Queries.GetPermissions
{
	public record GetPermissionsDto(
		Guid PermissionId,
		string EmployeeFullName,
		string PermissionTypeName,
		DateTime DateGranted
	);
}