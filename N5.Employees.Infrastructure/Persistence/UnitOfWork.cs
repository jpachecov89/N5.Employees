using N5.Employees.Application.Interfaces.Persistence;
using N5.Employees.Infrastructure.Persistence.Contexts;

namespace N5.Employees.Infrastructure.Persistence
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly AppDbContext _context;

		public IEmployeeRepository Employees { get; }
		public IPermissionRepository Permissions { get; }
		public IPermissionTypeRepository PermissionTypes { get; }

		public UnitOfWork(
			AppDbContext context,
			IEmployeeRepository employeeRepository,
			IPermissionRepository permissionRepository,
			IPermissionTypeRepository permissionTypeRepository)
		{
			_context = context;
			Employees = employeeRepository;
			Permissions = permissionRepository;
			PermissionTypes = permissionTypeRepository;
		}

		public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			return await _context.SaveChangesAsync(cancellationToken);
		}
	}
}