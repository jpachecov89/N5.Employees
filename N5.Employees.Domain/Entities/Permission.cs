namespace N5.Employees.Domain.Entities
{
	public class Permission
	{
		public Guid Id { get; set; }
		public Guid EmployeeId { get; set; }
		public Guid PermissionTypeId { get; set; }
		public DateTime DateGranted { get; set; }

		public Employee Employee { get; set; } = null!;
		public PermissionType PermissionType { get; set; } = null!;

		public Permission(Guid employeeId, Guid permissionTypeId, DateTime dateGranted)
		{
			Id = Guid.NewGuid();
			EmployeeId = employeeId;
			PermissionTypeId = permissionTypeId;
			DateGranted = dateGranted;
		}
	}
}