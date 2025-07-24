namespace N5.Employees.Domain.Entities
{
	public class Employee
	{
		public Guid Id { get; set; }
		public string FirstName { get; set; } = null!;
		public string LastName { get; set; } = null!;

		public string FullName => $"{FirstName} {LastName}";

		public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
	}
}