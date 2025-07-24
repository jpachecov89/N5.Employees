using Microsoft.EntityFrameworkCore;
using N5.Employees.Application.Interfaces.Persistence;
using N5.Employees.Domain.Entities;
using N5.Employees.Infrastructure.Persistence.Contexts;

namespace N5.Employees.Infrastructure.Persistence.Repositories
{
	public class EmployeeRepository : IEmployeeRepository
	{
		private readonly AppDbContext _context;

		public EmployeeRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task AddAsync(Employee employee) =>
			await _context.Employees.AddAsync(employee);

		public void Update(Employee employee) =>
			_context.Employees.Update(employee);

		public void Delete(Employee employee) =>
			_context.Employees.Remove(employee);

		public async Task<Employee?> GetByIdAsync(Guid id) =>
			await _context.Employees.FindAsync(id);

		public async Task<List<Employee>> GetAllAsync() =>
			await _context.Employees.ToListAsync();

		public async Task<bool> ExistsAsync(Guid id) =>
			await _context.Employees.AnyAsync(e => e.Id == id);
	}
}