using Microsoft.EntityFrameworkCore;
using N5.Employees.Application.Interfaces.Persistence;
using N5.Employees.Domain.Entities;
using N5.Employees.Infrastructure.Persistence.Contexts;

namespace N5.Employees.Infrastructure.Persistence.Repositories
{
	public class PermissionRepository : IPermissionRepository
	{
		private readonly AppDbContext _context;

		public PermissionRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task AddAsync(Permission permission) =>
			await _context.Permissions.AddAsync(permission);

		public void Update(Permission permission) =>
			_context.Permissions.Update(permission);

		public void Delete(Permission permission) =>
			_context.Permissions.Remove(permission);

		public async Task<Permission?> GetByIdAsync(Guid id) =>
			await _context.Permissions.FindAsync(id);

		public async Task<List<Permission>> GetAllAsync() =>
			await _context.Permissions
			.Include(x => x.Employee)
			.Include(x => x.PermissionType)
			.ToListAsync();

		public async Task<bool> ExistsAsync(Guid id) =>
			await _context.Permissions.AnyAsync(p => p.Id == id);
	}
}