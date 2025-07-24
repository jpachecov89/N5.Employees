using MediatR;

namespace N5.Employees.Application.Permissions.Queries.GetPermissions
{
	public record GetPermissionsQuery(
		Guid? PermissionTypeId,
		string? PermissionTypeName,
		Guid? EmployeeId,
		string? EmployeeName,
		DateTime? FromDate,
		DateTime? ToDate
	) : IRequest<List<GetPermissionsDto>>;
}