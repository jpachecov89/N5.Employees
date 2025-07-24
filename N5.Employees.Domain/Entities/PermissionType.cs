namespace N5.Employees.Domain.Entities
{
	public class PermissionType
	{
		public Guid Id { get; set; }
		public string Description { get; set; } = null!;

		public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
	}
}