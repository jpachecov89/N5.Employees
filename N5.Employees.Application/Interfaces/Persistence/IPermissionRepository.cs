using N5.Employees.Domain.Entities;

namespace N5.Employees.Application.Interfaces.Persistence
{
	public interface IPermissionRepository
	{
		Task<Permission?> GetByIdAsync(Guid id);
		Task<List<Permission>> GetAllAsync();
		Task AddAsync(Permission permission);
		void Update(Permission permission);
		void Delete(Permission permission);
		Task<bool> ExistsAsync(Guid id);
	}
}