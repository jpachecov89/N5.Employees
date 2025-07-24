using Microsoft.EntityFrameworkCore;
using N5.Employees.Application.Interfaces.Persistence;
using N5.Employees.Domain.Entities;
using N5.Employees.Infrastructure.Persistence.Contexts;

namespace N5.Employees.Infrastructure.Persistence.Repositories
{
	public class PermissionTypeRepository : IPermissionTypeRepository
	{
		private readonly AppDbContext _context;

		public PermissionTypeRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task AddAsync(PermissionType permissionType) =>
			await _context.PermissionTypes.AddAsync(permissionType);

		public void Update(PermissionType permissionType) =>
			_context.PermissionTypes.Update(permissionType);

		public void Delete(PermissionType permissionType) =>
			_context.PermissionTypes.Remove(permissionType);

		public async Task<PermissionType?> GetByIdAsync(Guid id) =>
			await _context.PermissionTypes.FindAsync(id);

		public async Task<List<PermissionType>> GetAllAsync() =>
			await _context.PermissionTypes.ToListAsync();

		public async Task<bool> ExistsAsync(Guid id) =>
			await _context.PermissionTypes.AnyAsync(p => p.Id == id);
	}
}