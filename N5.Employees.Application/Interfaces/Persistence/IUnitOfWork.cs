namespace N5.Employees.Application.Interfaces.Persistence
{
	public interface IUnitOfWork
	{
		IEmployeeRepository Employees { get; }
		IPermissionRepository Permissions { get; }
		IPermissionTypeRepository PermissionTypes { get; }

		Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
	}
}