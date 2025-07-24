using N5.Employees.Domain.Entities;

namespace N5.Employees.Application.Interfaces.Persistence
{
	public interface IPermissionTypeRepository
	{
		Task<PermissionType?> GetByIdAsync(Guid id);
		Task<List<PermissionType>> GetAllAsync();
		Task AddAsync(PermissionType permissionType);
		void Update(PermissionType permissionType);
		void Delete(PermissionType permissionType);
		Task<bool> ExistsAsync(Guid id);
	}
}