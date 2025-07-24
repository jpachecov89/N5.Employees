using Microsoft.EntityFrameworkCore;
using N5.Employees.Domain.Entities;

namespace N5.Employees.Infrastructure.Persistence.Contexts
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options)
			: base(options)
		{
		}

		public DbSet<Employee> Employees => Set<Employee>();
		public DbSet<Permission> Permissions => Set<Permission>();
		public DbSet<PermissionType> PermissionTypes => Set<PermissionType>();

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Employee>(entity =>
			{
				entity.HasData(
					new Employee { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" },
					new Employee { Id = Guid.NewGuid(), FirstName = "Ronald", LastName = "Reik" },
					new Employee { Id = Guid.NewGuid(), FirstName = "Michael", LastName = "Scout" }
				);
			});

			modelBuilder.Entity<PermissionType>(entity =>
			{
				entity.HasData(
					new PermissionType { Id = Guid.NewGuid(), Description = "Paid Vacations" },
					new PermissionType { Id = Guid.NewGuid(), Description = "No Paid Vacations" },
					new PermissionType { Id = Guid.NewGuid(), Description = "Change Schedule" },
					new PermissionType { Id = Guid.NewGuid(), Description = "Remote Work" },
					new PermissionType { Id = Guid.NewGuid(), Description = "Study License" }
				);
			});

			base.OnModelCreating(modelBuilder);
		}
	}
}