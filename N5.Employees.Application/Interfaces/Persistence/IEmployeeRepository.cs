using N5.Employees.Domain.Entities;

namespace N5.Employees.Application.Interfaces.Persistence
{
	public interface IEmployeeRepository
	{
		Task<Employee?> GetByIdAsync(Guid id);
		Task<List<Employee>> GetAllAsync();
		Task AddAsync(Employee employee);
		void Update(Employee employee);
		void Delete(Employee employee);
		Task<bool> ExistsAsync(Guid id);
	}
}
